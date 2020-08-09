using ASKON_TestTask.Common.Exceptions;
using ASKON_TestTask.Domain;
using ASKON_TestTask.Persistence;
using ASKON_TestTask.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASKON_TestTask.Services
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
                .Where(x => x.HierarchyLevel.GetLevel() == 1);


            var firstGenDetailIdsQuery = _context.Details
                .Join(firstGenRelationsQuery,
                    detail => detail.DetailId,
                    firstGenRelation => firstGenRelation.DetailId,
                    (detail, firstGenRelation) => new DetailInTreeView()
                    {
                        DetailId = detail.DetailId,
                        Name = detail.Name,
                        HierarchyLevel = firstGenRelation.HierarchyLevel
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
                .Where(x => ancestorsIds.Contains(x.HierarchyLevel.GetAncestor(1)));

            var targetDetailsWithRelations = await _context.Details
                .Join(targetRelationsQuery,
                    detail => detail.DetailId, targetRelation => targetRelation.DetailId,
                    (detail, targetRelation) => new DetailInTreeView()
                    {
                        DetailId = detail.DetailId,
                        Name = detail.Name,
                        Count = targetRelation.Count,
                        HierarchyLevel = targetRelation.HierarchyLevel
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

        public async Task DeleteDetailWithDescendantsAsync(HierarchyId detailHierarchyId)
        {
            await using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var descendantsRelationsQuery = _context.DetailRelations
                        .Where(x => x.HierarchyLevel.IsDescendantOf(detailHierarchyId));

                    var descendantsDetailsIds = await descendantsRelationsQuery
                        .AsNoTracking()
                        .Select(x => x.DetailId)
                        .Distinct()
                        .ToListAsync()
                        .ConfigureAwait(false);

                    _context.DetailRelations.RemoveRange(descendantsRelationsQuery);

                    await _context.SaveChangesAsync().ConfigureAwait(false);

                    var detailIdsWithRelations = _context.DetailRelations
                        .Select(x => x.DetailId).Distinct();

                    var detailsToRemove = _context.Details
                        .Where(x => descendantsDetailsIds.Contains(x.DetailId) &&
                                    !detailIdsWithRelations.Contains(x.DetailId));

                    _context.Details.RemoveRange(detailsToRemove);

                    await _context.SaveChangesAsync().ConfigureAwait(false);

                    await transaction.CommitAsync().ConfigureAwait(false);
                }
                catch (Exception)
                {
                }
            }
        }

        public async Task<List<DetailForReport>> GetDetailsForReportAsync(
            int rootDetailId,
            HierarchyId rootHierarchyId)
        {
            var childRelations = await _context.DetailRelations
                .Include(x => x.Detail)
                .Where(x => x.HierarchyLevel.IsDescendantOf(rootHierarchyId))
                .AsNoTracking()
                .ToListAsync()
                .ConfigureAwait(false);

            childRelations
                .ForEach(x => childRelations
                    .ForEach(y =>
                        {
                            if (x.DetailId != rootDetailId &&
                                y.DetailId != x.DetailId &&
                                y.HierarchyLevel.GetAncestor(1) == x.HierarchyLevel)
                            {
                                y.Count = x.Count != null ? y.Count * x.Count : y.Count;
                            }
                        }
                    ));

            var ancestorNodesIds = childRelations
                .Select(x => x.HierarchyLevel.GetAncestor(1));

            childRelations
                .RemoveAll(x => x.Count == null ||
                                ancestorNodesIds.Contains(x.HierarchyLevel));

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

        public async Task<DetailInTreeView> AddParentDetailAsync(string detailName)
        {
            await using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var isUniqueName = await IsUniqueDetailNameAsync(detailName);

                    if (!isUniqueName)
                    {
                        return null;
                    }

                    var addedDetail = _context.Details
                        .Add(new Detail
                        {
                            Name = detailName
                        });

                    await _context.SaveChangesAsync().ConfigureAwait(false);

                    var addedRelation = _context.DetailRelations
                        .Add(new DetailRelation
                        {
                            DetailId = addedDetail.Entity.DetailId,
                            HierarchyLevel = HierarchyId.Parse($"/{addedDetail.Entity.DetailId}/")
                        });

                    await _context.SaveChangesAsync().ConfigureAwait(false);

                    var detailEntity = new DetailInTreeView()
                    {
                        DetailId = addedDetail.Entity.DetailId,
                        Name = addedDetail.Entity.Name,
                        HierarchyLevel = addedRelation.Entity.HierarchyLevel
                    };

                    await transaction.CommitAsync().ConfigureAwait(false);

                    return detailEntity;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public async Task<List<DetailInTreeView>> AddChildDetailAsync(
            int parentDetailId,
            HierarchyId parentHierarchyId,
            List<HierarchyId> childHierarchyIds,
            string childDetailName,
            int countToAdd)
        {
            var detailsIdsForbiddenToAdd = new List<int>
            {
                parentDetailId
            };

            await using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                var retrievedDetail =
                    await _context.Details
                        .AsNoTracking()
                        .SingleOrDefaultAsync(x => x.Name == childDetailName)
                        .ConfigureAwait(false);

                HierarchyId hierarchyIdToAdd;

                List<DetailInTreeView> childDetailsInTree = null;

                // Handle case when there is already a detail with such name in db
                if (retrievedDetail != null)
                {
                    var selectedNodeLevel = parentHierarchyId.GetLevel();

                    var ancestorsLevels = new List<HierarchyId>();

                    for (var i = 1; i < selectedNodeLevel; i++)
                    {
                        ancestorsLevels.Add(parentHierarchyId.GetAncestor(i));
                    }

                    var retrievedForbiddenIds = await _context.DetailRelations
                        .AsNoTracking()
                        .Where(x => ancestorsLevels.Contains(x.HierarchyLevel))
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
                        .Where(x => x.HierarchyLevel.IsDescendantOf(existedDetailRelation.HierarchyLevel))
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

                    hierarchyIdToAdd = HierarchyId.Parse(
                        parentHierarchyId.ToString() +
                        $"{retrievedDetail.DetailId}/");


                    // Handle case when detail is already present in the selected scope
                    if (childHierarchyIds != null)
                    {
                        foreach (var childHierarchyId in childHierarchyIds)
                        {
                            if (childHierarchyId.Equals(hierarchyIdToAdd))
                            {
                                var relationToUpdate =
                                    await _context.DetailRelations
                                        .SingleOrDefaultAsync(x =>
                                            x.HierarchyLevel.Equals(hierarchyIdToAdd));

                                if (relationToUpdate != null)
                                {
                                    relationToUpdate.Count += countToAdd;
                                }

                                await _context.SaveChangesAsync().ConfigureAwait(false);

                                var result = new List<DetailInTreeView>
                                {
                                    new DetailInTreeView
                                    {
                                        DetailId = relationToUpdate.DetailId,
                                        Name = childDetailName,
                                        Count = relationToUpdate.Count,
                                        HierarchyLevel = relationToUpdate.HierarchyLevel
                                    }
                                };

                                return result;
                            }
                        }
                    }

                    // Handle case when detail is NOT present in the selected scope

                    var parentHierarchyIdToChange = existedDetailRelation.HierarchyLevel.GetAncestor(1);

                    var childRelationsToAdd = new List<DetailRelation>();

                    existedChildRelations.ForEach(x =>
                    {
                        childRelationsToAdd
                            .Add(
                                new DetailRelation
                                {
                                    DetailId = x.DetailId,
                                    HierarchyLevel = x.HierarchyLevel
                                        .GetReparentedValue(parentHierarchyIdToChange, parentHierarchyId),
                                    Count = x.DetailId == retrievedDetail.DetailId ? countToAdd : x.Count
                                });
                    });


                    var rs = 1;

                    _context.DetailRelations
                        .AddRange(childRelationsToAdd);

                    await _context.SaveChangesAsync().ConfigureAwait(false);

                    var hierarchyIdsOfAddedRelations =
                        childRelationsToAdd.Select(x => x.HierarchyLevel);

                    var resultListOfDetails = await _context.DetailRelations
                        .AsNoTracking()
                        .Include(x => x.Detail)
                        .Where(x => hierarchyIdsOfAddedRelations.Contains(x.HierarchyLevel))
                        .Select(x => new DetailInTreeView
                        {
                            DetailId = x.DetailId,
                            Name = x.Detail.Name,
                            Count = x.Count,
                            HierarchyLevel = x.HierarchyLevel
                        })
                        .ToListAsync()
                        .ConfigureAwait(false);

                    childDetailsInTree = resultListOfDetails;
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

                    hierarchyIdToAdd = HierarchyId.Parse(
                        parentHierarchyId.ToString() + $"{addedDetail.Entity.DetailId}/");

                    var addedRelation = _context.DetailRelations.Add(
                        new DetailRelation
                        {
                            DetailId = addedDetail.Entity.DetailId,
                            Count = countToAdd,
                            HierarchyLevel = hierarchyIdToAdd
                        });

                    await _context.SaveChangesAsync().ConfigureAwait(false);


                    childDetailsInTree = new List<DetailInTreeView>
                    {
                        new DetailInTreeView
                        {
                            DetailId = addedDetail.Entity.DetailId,
                            Name = addedDetail.Entity.Name,
                            Count = addedRelation.Entity.Count,
                            HierarchyLevel = addedRelation.Entity.HierarchyLevel
                        }
                    };
                }

                await transaction.CommitAsync().ConfigureAwait(false);

                return childDetailsInTree;
            }
        }
    }
}
