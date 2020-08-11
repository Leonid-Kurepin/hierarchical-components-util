using Microsoft.EntityFrameworkCore;

namespace ComponentsUtil.Persistence.Entities
{
    public class DetailRelation
    {
        public int DetailRelationId { get; set; }
        public int DetailId { get; set; }
        public int? Count { get; set; }

        public HierarchyId HierarchyId { get; set; }

        public Detail Detail { get; set; }
    }
}
