using System.Collections.Generic;

namespace ComponentsUtil.Persistence.Entities
{
    public class Detail
    {
        public Detail()
        {
            DetailRelations = new HashSet<DetailRelation>();
        }

        public int DetailId { get; set; }
        public string Name { get; set; }

        public ICollection<DetailRelation> DetailRelations { get; set; }
    }
}
