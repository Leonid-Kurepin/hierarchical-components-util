using ASKON_TestTask.Domain;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASKON_TestTask.Services
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
        /// Deletes detail and its descendants by the detail HierarchyId.
        /// Removes related rows in the DetailRelations table,
        /// and removes details from the Details table if there
        /// are relations left for the deleting details.
        /// </summary>
        /// <param name="detailHierarchyId">HierarchyId of parent detail.</param>
        public Task DeleteDetailWithDescendantsAsync(HierarchyId detailHierarchyId);

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
        /// <returns>Created detail entity.</returns>
        public Task<DetailInTreeView> AddParentDetailAsync(string detailName);


        /// <summary>
        /// Adds child detail with all its descendants to the selected parent detail.
        /// </summary>
        /// <param name="parentDetailId">Id of selected parent detail.</param>
        /// <param name="parentHierarchyId">HierarchyId of selected parent detail.</param>
        /// <param name="childHierarchyIds">List of HierarchyIds of the selected parent detail children.</param>
        /// <param name="childDetailName">Name of added child detail.</param>
        /// <param name="countToAdd">Count to add.</param>
        /// <returns>List of created details entities.</returns>
        public Task<List<DetailInTreeView>> AddChildDetailAsync(
            int parentDetailId,
            HierarchyId parentHierarchyId,
            List<HierarchyId> childHierarchyIds,
            string childDetailName,
            int countToAdd);
    }
}
