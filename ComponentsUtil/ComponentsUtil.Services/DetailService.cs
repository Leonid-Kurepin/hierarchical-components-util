using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ComponentsUtil.Common.Exceptions;
using ComponentsUtil.Domain;
using ComponentsUtil.Persistence;
using ComponentsUtil.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace ComponentsUtil.Services
{
    public class DetailService : IDetailService
    {
        private readonly TestTaskContext _context;

        public DetailService(TestTaskContext context)
        {
            _context = context;
        }

        public async Task<List<DetailInTreeView>> GetParentDetailsAsync()
        {
            var firstGenRelationsQuery = _context.DetailRelations
                .Where(x => x.HierarchyId.GetLevel() == 1);


            var firstGenDetailIdsQuery = _context.Details
                .Join(firstGenRelationsQuery,
                    detail => detail.DetailId,
                    firstGenRelation => firstGenRelation.DetailId,
                    (detail, firstGenRelation) => new DetailInTreeView()
                    {
                        DetailId = detail.DetailId,
                        Name = detail.Name,
                        HierarchyLevel = firstGenRelation.HierarchyId
                    });

            var firstGenDetails = await firstGenDetailIdsQuery
                .AsNoTracking()
                .ToListAsync()
                .ConfigureAwait(false);

            return firstGenDetails;
        }

        public async Task<List<DetailInTreeView>> GetDetailsFirstDescendantsAsync(
            List<HierarchyId> ancestorsIds)
        {
            var targetRelationsQuery = _context.DetailRelations
                .Where(x => ancestorsIds.Contains(x.HierarchyId.GetAncestor(1)));

            var targetDetailsWithRelations = await _context.Details
                .Join(targetRelationsQuery,
                    detail => detail.DetailId, targetRelation => targetRelation.DetailId,
                    (detail, targetRelation) => new DetailInTreeView()
                    {
                        DetailId = detail.DetailId,
                        Name = detail.Name,
                        Count = targetRelation.Count,
                        HierarchyLevel = targetRelation.HierarchyId
                    })
                .OrderBy(x => x.Name)
                .AsNoTracking()
                .ToListAsync()
                .ConfigureAwait(false);

            return targetDetailsWithRelations;
        }

        public async Task<bool> IsUniqueDetailNameAsync(string detailName)
        {
            var isUniqueName = !await _context.Details
                .AnyAsync(x => x.Name == detailName)
                .ConfigureAwait(false);

            return isUniqueName;
        }

        public async Task<DetailInTreeView> UpdateDetailNameAsync(int detailId, string newDetailName)
        {
            var retrievedDetailToUpdate = await _context.Details
                .SingleOrDefaultAsync(x => x.DetailId == detailId)
                .ConfigureAwait(false);

            if (retrievedDetailToUpdate == null)
            {
                return null;
            }

            var isUniqueName = await IsUniqueDetailNameAsync(newDetailName);

            if (!isUniqueName)
            {
                return null;
            }

            retrievedDetailToUpdate.Name = newDetailName;

            await _context.SaveChangesAsync().ConfigureAwait(false);

            return new DetailInTreeView
            {
                DetailId = retrievedDetailToUpdate.DetailId,
                Name = retrievedDetailToUpdate.Name
            };
        }

        public async Task<List<HierarchyId>> DeleteDetailWithDescendantsAsync(
            int detailId,
            int? parentDetailId)
        {
            List<DetailRelation> childRelationsToDelete = null;

            await using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                List<HierarchyId> parentHierarchyIds = null;

                if (parentDetailId != null)
                {
                    parentHierarchyIds = await GetDetailHierarchyIdsAsync(parentDetailId.Value);
                }
                else
                {
                    parentHierarchyIds = new List<HierarchyId>
                    {
                        HierarchyId.GetRoot()
                    };
                }

                var detailRelations = await _context.DetailRelations
                    .AsNoTracking()
                    .Where(x => x.DetailId == detailId)
                    .ToListAsync()
                    .ConfigureAwait(false);

                var ancestorRelationsToDelete = new List<DetailRelation>();

                detailRelations.ForEach(detailRelation =>
                {
                    if (parentHierarchyIds.Contains(
                        detailRelation.HierarchyId.GetAncestor(1)))
                    {
                        ancestorRelationsToDelete.Add(detailRelation);
                    }
                });

                var ancestorHierarchyIdsToDelete = ancestorRelationsToDelete
                    .Select(x => x.HierarchyId)
                    .ToList();

                IQueryable<DetailRelation> childRelationsToDeleteQuery = _context.DetailRelations
                    .Where(x => false);

                ancestorHierarchyIdsToDelete.ForEach(ancestorHierarchyId =>
                {
                    var queryPart = _context.DetailRelations
                        .Where(x => x.HierarchyId
                            .IsDescendantOf(ancestorHierarchyId));

                    childRelationsToDeleteQuery = childRelationsToDeleteQuery.Concat(queryPart);
                });


                childRelationsToDelete = await childRelationsToDeleteQuery
                    .ToListAsync()
                    .ConfigureAwait(false);

                // To address: For some reason added DetailRelations are being tracked by the context
                // if use RemoveRange method, then delete query fails or don't work properly

                //_context.DetailRelations.RemoveRange(childRelationsToDelete);

                childRelationsToDelete.ForEach(x =>
                {
                    var entry = _context.Entry(x);
                    entry.State = EntityState.Deleted;
                });

                await _context.SaveChangesAsync().ConfigureAwait(false);

                var detailIdsOfDeletedRelations = childRelationsToDelete
                    .Select(x => x.DetailId)
                    .Distinct()
                    .ToList();

                var detailIdsToNotDelete = await _context.DetailRelations
                    .Where(x => detailIdsOfDeletedRelations.Contains(x.DetailId))
                    .Select(x => x.DetailId)
                    .Distinct()
                    .ToListAsync()
                    .ConfigureAwait(false);

                var detailsIdsToRemove = detailIdsOfDeletedRelations
                    .Except(detailIdsToNotDelete)
                    .ToList();

                if (detailsIdsToRemove.Any())
                {
                    var detailsToRemove = _context.Details
                        .Where(x => detailsIdsToRemove.Contains(x.DetailId));

                    _context.Details.RemoveRange(detailsToRemove);

                    await _context.SaveChangesAsync().ConfigureAwait(false);
                }

                await transaction.CommitAsync().ConfigureAwait(false);
            }

            var resultListOfHierarchyIds = childRelationsToDelete?
                .Select(x => x.HierarchyId).ToList();

            return resultListOfHierarchyIds;
        }

        public async Task<List<DetailForReport>> GetDetailsForReportAsync(
            int rootDetailId,
            HierarchyId rootHierarchyId)
        {
            var childRelations = await _context.DetailRelations
                .Include(x => x.Detail)
                .Where(x => x.HierarchyId.IsDescendantOf(rootHierarchyId))
                .AsNoTracking()
                .ToListAsync()
                .ConfigureAwait(false);

            childRelations
                .ForEach(x => childRelations
                    .ForEach(y =>
                        {
                            if (x.DetailId != rootDetailId &&
                                y.DetailId != x.DetailId &&
                                y.HierarchyId.GetAncestor(1) == x.HierarchyId)
                            {
                                y.Count = x.Count != null ? y.Count * x.Count : y.Count;
                            }
                        }
                    ));

            var ancestorNodesIds = childRelations
                .Select(x => x.HierarchyId.GetAncestor(1));

            childRelations
                .RemoveAll(x => x.Count == null ||
                                ancestorNodesIds.Contains(x.HierarchyId));

            var detailsForReport = childRelations
                .GroupBy(x => x.Detail.Name)
                .Select(
                    g => new DetailForReport
                    {
                        DetailName = g.First().Detail.Name,
                        Count = g.Sum(dr => dr.Count)
                    })
                .OrderBy(x => x.DetailName)
                .ToList();

            return detailsForReport;
        }

        public async Task<List<DetailInTreeView>> AddParentDetailAsync(string detailName)
        {
            List<DetailInTreeView> resultListOfDetails = null;

            await using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                var retrievedDetail =
                    await _context.Details
                        .AsNoTracking()
                        .SingleOrDefaultAsync(x => x.Name == detailName)
                        .ConfigureAwait(false);


                // Handle case when there is already a detail with such name in db
                if (retrievedDetail != null)
                {
                    var existedDetailRelations = await _context.DetailRelations
                        .AsNoTracking()
                        .Where(x => x.DetailId == retrievedDetail.DetailId)
                        .ToListAsync()
                        .ConfigureAwait(false);

                    var isRootDetail = existedDetailRelations.Any(x => x.Count == null);

                    if (isRootDetail)
                    {
                        var exceptionMessage = $"Detail with name \'{detailName}\' " +
                                               $"\n\ris already added as parent detail!";

                        throw new BusinessLogicException(exceptionMessage);
                    }

                    var existedDetailRelation = existedDetailRelations.FirstOrDefault();

                    var existedChildRelations = await _context.DetailRelations
                        .AsNoTracking()
                        .Where(x => x.HierarchyId
                            .IsDescendantOf(existedDetailRelation.HierarchyId))
                        .ToListAsync()
                        .ConfigureAwait(false);

                    var parentHierarchyIdToChange = existedDetailRelation.HierarchyId.GetAncestor(1);

                    var newParentHierarchyId = HierarchyId.GetRoot();

                    var childRelationsToAdd = new List<DetailRelation>();

                    existedChildRelations.ForEach(x =>
                    {
                        childRelationsToAdd
                            .Add(
                                new DetailRelation
                                {
                                    DetailId = x.DetailId,
                                    HierarchyId = x.HierarchyId
                                        .GetReparentedValue(parentHierarchyIdToChange, newParentHierarchyId),
                                    Count = x.DetailId == retrievedDetail.DetailId ? null : x.Count
                                });
                    });

                    _context.DetailRelations
                        .AddRange(childRelationsToAdd);

                    await _context.SaveChangesAsync().ConfigureAwait(false);

                    var hierarchyIdsOfAddedRelations =
                        childRelationsToAdd.Select(x => x.HierarchyId);

                    resultListOfDetails = await _context.DetailRelations
                        .AsNoTracking()
                        .Include(x => x.Detail)
                        .Where(x => hierarchyIdsOfAddedRelations.Contains(x.HierarchyId))
                        .Select(x => new DetailInTreeView
                        {
                            DetailId = x.DetailId,
                            Name = x.Detail.Name,
                            Count = x.Count,
                            HierarchyLevel = x.HierarchyId
                        })
                        .ToListAsync()
                        .ConfigureAwait(false);
                }

                // Handle case when there is no detail with such name in db
                if (retrievedDetail == null)
                {
                    var addedDetail = _context.Details.Add(
                        new Detail
                        {
                            Name = detailName
                        });

                    await _context.SaveChangesAsync().ConfigureAwait(false);

                    var hierarchyIdToAdd = HierarchyId.Parse($"/{addedDetail.Entity.DetailId}/");

                    var addedRelation = _context.DetailRelations.Add(
                        new DetailRelation
                        {
                            DetailId = addedDetail.Entity.DetailId,
                            Count = null,
                            HierarchyId = hierarchyIdToAdd
                        });

                    await _context.SaveChangesAsync().ConfigureAwait(false);

                    resultListOfDetails = new List<DetailInTreeView>
                    {
                        new DetailInTreeView
                        {
                            DetailId = addedDetail.Entity.DetailId,
                            Name = addedDetail.Entity.Name,
                            Count = addedRelation.Entity.Count,
                            HierarchyLevel = addedRelation.Entity.HierarchyId
                        }
                    };
                }

                await transaction.CommitAsync().ConfigureAwait(false);
            }

            return resultListOfDetails;
        }

        public async Task<List<DetailInTreeView>> AddChildDetailAsync(
            int parentDetailId,
            string childDetailName,
            int countToAdd)
        {
            var detailsIdsForbiddenToAdd = new List<int>
            {
                parentDetailId
            };

            var resultListOfDetails = new List<DetailInTreeView>();

            await using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                var retrievedDetail =
                    await _context.Details
                        .AsNoTracking()
                        .SingleOrDefaultAsync(x => x.Name == childDetailName)
                        .ConfigureAwait(false);


                var parentHierarchyIds = await GetDetailHierarchyIdsAsync(parentDetailId);

                // Handle case when there is already a detail with such name in db
                if (retrievedDetail != null)
                {
                    var ancestorsLevels = new List<HierarchyId>();

                    parentHierarchyIds.ForEach(x =>
                    {
                        var selectedNodeLevel = x.GetLevel();

                        for (var i = 1; i < selectedNodeLevel; i++)
                        {
                            ancestorsLevels.Add(x.GetAncestor(i));
                        }
                    });

                    var retrievedForbiddenIds = await _context.DetailRelations
                        .AsNoTracking()
                        .Where(x => ancestorsLevels.Contains(x.HierarchyId))
                        .Select(x => x.DetailId)
                        .ToListAsync()
                        .ConfigureAwait(false);

                    detailsIdsForbiddenToAdd.AddRange(retrievedForbiddenIds);

                    var existedDetailRelation = await _context.DetailRelations
                        .AsNoTracking()
                        .FirstOrDefaultAsync(x => x.DetailId == retrievedDetail.DetailId)
                        .ConfigureAwait(false);

                    var existedChildRelations = await _context.DetailRelations
                        .AsNoTracking()
                        .Where(x => x.HierarchyId.IsDescendantOf(existedDetailRelation.HierarchyId))
                        .ToListAsync()
                        .ConfigureAwait(false);


                    existedChildRelations.ForEach(x =>
                    {
                        if (detailsIdsForbiddenToAdd.Contains(x.DetailId))
                        {
                            var exceptionMessage = "You can\'t add the detail!" +
                                                   "\n\rRecursive dependency" +
                                                   "\n\ris not allowed.";

                            throw new BusinessLogicException(exceptionMessage);
                        }
                    });

                    var childHierarchyIds = await _context.DetailRelations
                        .AsNoTracking()
                        .Where(x => parentHierarchyIds
                            .Contains(x.HierarchyId.GetAncestor(1)))
                        .Select(x => x.HierarchyId)
                        .ToListAsync()
                        .ConfigureAwait(false);


                    var hierarchyIdsToAdd = new List<HierarchyId>();

                    parentHierarchyIds.ForEach(x =>
                    {
                        var hierarchyIdToAdd = HierarchyId.Parse(
                            x.ToString() +
                            $"{retrievedDetail.DetailId}/");

                        hierarchyIdsToAdd.Add(hierarchyIdToAdd);
                    });


                    // Handle case when detail is already present in the selected scope
                    if (childHierarchyIds != null)
                    {
                        var relationsToUpdate =
                            await _context.DetailRelations
                                .Where(x =>
                                    hierarchyIdsToAdd.Contains(x.HierarchyId))
                                .ToListAsync()
                                .ConfigureAwait(false);

                        if (relationsToUpdate.Any())
                        {
                            relationsToUpdate.ForEach(x =>
                            {
                                x.Count += countToAdd;

                                resultListOfDetails.Add(new DetailInTreeView
                                {
                                    DetailId = x.DetailId,
                                    Name = childDetailName,
                                    Count = x.Count,
                                    HierarchyLevel = x.HierarchyId
                                });
                            });

                            await _context.SaveChangesAsync().ConfigureAwait(false);

                            await transaction.CommitAsync().ConfigureAwait(false);

                            return resultListOfDetails;
                        }
                    }

                    // Handle case when detail is NOT present in the selected scope
                    var parentHierarchyIdToChange = existedDetailRelation.HierarchyId.GetAncestor(1);

                    var childRelationsToAdd = new List<DetailRelation>();

                    existedChildRelations.ForEach(x =>
                    {
                        parentHierarchyIds.ForEach(y =>
                        {
                            childRelationsToAdd
                                .Add(
                                    new DetailRelation
                                    {
                                        DetailId = x.DetailId,
                                        HierarchyId = x.HierarchyId
                                            .GetReparentedValue(parentHierarchyIdToChange, y),
                                        Count = x.DetailId == retrievedDetail.DetailId ? countToAdd : x.Count
                                    });
                        });
                    });

                    _context.DetailRelations
                        .AddRange(childRelationsToAdd);

                    await _context.SaveChangesAsync().ConfigureAwait(false);

                    var hierarchyIdsOfAddedRelations =
                        childRelationsToAdd.Select(x => x.HierarchyId);

                    resultListOfDetails = await _context.DetailRelations
                        .AsNoTracking()
                        .Include(x => x.Detail)
                        .Where(x => hierarchyIdsOfAddedRelations.Contains(x.HierarchyId))
                        .Select(x => new DetailInTreeView
                        {
                            DetailId = x.DetailId,
                            Name = x.Detail.Name,
                            Count = x.Count,
                            HierarchyLevel = x.HierarchyId
                        })
                        .ToListAsync()
                        .ConfigureAwait(false);

                    await transaction.CommitAsync().ConfigureAwait(false);

                    return resultListOfDetails;
                }

                // Handle case when there is no detail with such name in db
                if (retrievedDetail == null)
                {
                    var addedDetail = _context.Details.Add(
                        new Detail
                        {
                            Name = childDetailName
                        });

                    await _context.SaveChangesAsync().ConfigureAwait(false);

                    var parentDetailRelations = await _context.DetailRelations
                        .AsNoTracking()
                        .Where(x => x.DetailId == parentDetailId)
                        .ToListAsync()
                        .ConfigureAwait(false);

                    var relationsToAdd = new List<DetailRelation>();

                    parentDetailRelations.ForEach(x =>
                    {
                        var hierarchyIdToAdd = HierarchyId.Parse(
                            x.HierarchyId.ToString() + $"{addedDetail.Entity.DetailId}/");

                        var relationToAdd = new DetailRelation
                        {
                            DetailId = addedDetail.Entity.DetailId,
                            Count = countToAdd,
                            HierarchyId = hierarchyIdToAdd
                        };

                        relationsToAdd.Add(relationToAdd);
                    });

                    _context.AddRange(relationsToAdd);

                    await _context.SaveChangesAsync().ConfigureAwait(false);

                    relationsToAdd.ForEach(x =>
                    {
                        resultListOfDetails.Add(new DetailInTreeView
                        {
                            DetailId = x.DetailId,
                            Name = addedDetail.Entity.Name,
                            Count = x.Count,
                            HierarchyLevel = x.HierarchyId
                        });
                    });
                }

                await transaction.CommitAsync().ConfigureAwait(false);

            }

            return resultListOfDetails;
        }

        public async Task<List<HierarchyId>> GetDetailHierarchyIdsAsync(int detailId)
        {
            var detailHierarchyIds = await _context.DetailRelations
                .AsNoTracking()
                .Where(x => x.DetailId == detailId)
                .Select(x => x.HierarchyId)
                .ToListAsync()
                .ConfigureAwait(false);

            return detailHierarchyIds;
        }
    }
}
