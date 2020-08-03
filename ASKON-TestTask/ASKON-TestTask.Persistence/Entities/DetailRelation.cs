using Microsoft.EntityFrameworkCore;

namespace ASKON_TestTask.Persistence.Entities
{
    public class DetailRelation
    {
        public int DetailRelationId { get; set; }
        public int DetailId { get; set; }
        public int? Count { get; set; }

        public HierarchyId HierarchyLevel { get; set; }

        public Detail Detail { get; set; }
    }
}
