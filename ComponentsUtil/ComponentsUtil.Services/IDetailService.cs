using System.Collections.Generic;
using System.Threading.Tasks;
using ComponentsUtil.Domain;
using Microsoft.EntityFrameworkCore;

namespace ComponentsUtil.Services
{
    public interface IDetailService
    {
        /// <summary>
        /// Retrieves all parent details.
        /// </summary>
        /// <returns>List of parent details with their HierarchyId.</returns>
        public Task<List<DetailInTreeView>> GetParentDetailsAsync();

        /// <summary>
        /// Retrieves list of first generation descendant details for the given list of parents hierarchy ids.
        /// </summary>
        /// <param name="ancestorsIds">List of parents hierarchy ids.</param>
        /// <returns>List of descendant details with their Count and HierarchyId.</returns>
        public Task<List<DetailInTreeView>> GetDetailsFirstDescendantsAsync(List<HierarchyId> ancestorsIds);

        /// <summary>
        /// Verifies is given detail name unique or not.
        /// </summary>
        /// <param name="detailName">Detail name.</param>
        /// <returns></returns>
        public Task<bool> IsUniqueDetailNameAsync(string detailName);

        /// <summary>
        /// Retrieves detail by given id and updates its name.
        /// </summary>
        /// <param name="detailId">Detail id of updated detail.</param>
        /// <param name="newDetailName">New detail name.</param>
        /// <returns>Updated detail entity.</returns>
        public Task<DetailInTreeView> UpdateDetailNameAsync(
            int detailId,
            string newDetailName);

        /// <summary>
        /// Deletes detail and its descendants.
        /// Removes related rows in the DetailRelations table,
        /// and removes details from the Details table if there
        /// are no relations left for the deleting details.
        /// </summary>
        /// <param name="detailId">Id of detail being deleted.</param>
        /// <param name="detailParentId">Id of parent detail of the deleting detail.</param>
        /// <returns>List of HierarchyIds of removed details.</returns>
        public Task<List<HierarchyId>> DeleteDetailWithDescendantsAsync(
            int detailId,
            int? detailParentId);

        /// <summary>
        /// Composes list of details for report.
        /// </summary>
        /// <param name="rootDetailId">Id of the detail for which report is composed.</param>
        /// <param name="rootHierarchyId">HierarchyId of the detail for which report is composed.</param>
        /// <returns>List of details presented in the selected details.
        /// The list contains detail name and its count, presented in the selected detail.</returns>
        public Task<List<DetailForReport>> GetDetailsForReportAsync(
            int rootDetailId,
            HierarchyId rootHierarchyId);

        /// <summary>
        /// Adds parent detail with given name.
        /// </summary>
        /// <param name="detailName">Name of created detail.</param>
        /// <returns>List of parent detail entity with its children (if exists).</returns>
        public Task<List<DetailInTreeView>> AddParentDetailAsync(string detailName);


        /// <summary>
        /// Adds child detail with all its descendants to the selected parent detail.
        /// </summary>
        /// <param name="parentDetailId">Id of selected parent detail.</param>
        /// <param name="childDetailName">Name of added child detail.</param>
        /// <param name="countToAdd">Count to add.</param>
        /// <returns>List of created details entities.</returns>
        public Task<List<DetailInTreeView>> AddChildDetailAsync(
            int parentDetailId,
            string childDetailName,
            int countToAdd);

        /// <summary>
        /// Retrieves all the detail HierarchyIds of the selected detail.
        /// </summary>
        /// <param name="detailId">Id of the selected detail.</param>
        /// <returns>List of all the detail HierarchyId.</returns>
        public Task<List<HierarchyId>> GetDetailHierarchyIdsAsync(
            int detailId);
    }
}
