using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace ASKON_TestTask.Domain
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
