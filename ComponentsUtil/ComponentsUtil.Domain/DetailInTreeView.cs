using Microsoft.EntityFrameworkCore;

namespace ComponentsUtil.Domain
{
    public class DetailInTreeView
    {
        public int DetailId { get; set; }
        public string Name { get; set; }
        public int? Count { get; set; }
        public HierarchyId HierarchyLevel { get; set; }

        public bool IsThirdGenLoaded { get; set; }
    }
}
