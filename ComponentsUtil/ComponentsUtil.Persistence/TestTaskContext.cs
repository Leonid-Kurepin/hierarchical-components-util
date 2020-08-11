using ComponentsUtil.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace ComponentsUtil.Persistence
{
    public class TestTaskContext : DbContext
    {
        public TestTaskContext(DbContextOptions<TestTaskContext> options)
            : base(options)
        {
        }

        public TestTaskContext()
        {
        }

        public DbSet<Detail> Details { get; set; }
        public DbSet<DetailRelation> DetailRelations { get; set; }

        /*
        public virtual DbSet<DetailForReport> DetailsForReport { get; set; }
        */

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AssemblyMarker).Assembly);

            /*
            modelBuilder.Entity<DetailForReport>(entity =>
            {
                entity.HasNoKey();
                entity.Property(e => e.DetailName);
                entity.Property(e => e.Count);
            });
            */

            #region TestData

            modelBuilder.Entity<Detail>().HasData
            (
                new Detail()
                {
                    DetailId = 1,
                    Name = "Engine #234"
                },
                new Detail()
                {
                    DetailId = 2,
                    Name = "Engine #456"
                },
                new Detail()
                {
                    DetailId = 3,
                    Name = "Crankshaft"
                },
                new Detail()
                {
                    DetailId = 4,
                    Name = "Camshaft"
                },
                new Detail()
                {
                    DetailId = 5,
                    Name = "Piston assembly"
                },
                new Detail()
                {
                    DetailId = 6,
                    Name = "Piston"
                },
                new Detail()
                {
                    DetailId = 7,
                    Name = "Compression ring"
                },
                new Detail()
                {
                    DetailId = 8,
                    Name = "Oil scrapper ring"
                },
                new Detail()
                {
                    DetailId = 9,
                    Name = "Connecting rod"
                },
                new Detail()
                {
                    DetailId = 10,
                    Name = "Cylinder block"
                }
            );

            modelBuilder.Entity<DetailRelation>().HasData
            (
                new DetailRelation()
                {
                    DetailRelationId = 1,
                    DetailId = 1,
                    Count = null,
                    HierarchyId = HierarchyId.Parse("/1/")
                },
                new DetailRelation()
                {
                    DetailRelationId = 2,
                    DetailId = 2,
                    Count = null,
                    HierarchyId = HierarchyId.Parse("/2/")
                },
                new DetailRelation()
                {
                    DetailRelationId = 3,
                    DetailId = 10,
                    Count = 1,
                    HierarchyId = HierarchyId.Parse("/1/10/")
                },
                new DetailRelation()
                {
                    DetailRelationId = 4,
                    DetailId = 3,
                    Count = 1,
                    HierarchyId = HierarchyId.Parse("/1/3/")
                },
                new DetailRelation()
                {
                    DetailRelationId = 5,
                    DetailId = 4,
                    Count = 2,
                    HierarchyId = HierarchyId.Parse("/1/4/")
                },
                new DetailRelation()
                {
                    DetailRelationId = 6,
                    DetailId = 5,
                    Count = 4,
                    HierarchyId = HierarchyId.Parse("/1/5/")
                },
                new DetailRelation()
                {
                    DetailRelationId = 7,
                    DetailId = 6,
                    Count = 1,
                    HierarchyId = HierarchyId.Parse("/1/5/6/")
                },
                new DetailRelation()
                {
                    DetailRelationId = 8,
                    DetailId = 7,
                    Count = 2,
                    HierarchyId = HierarchyId.Parse("/1/5/7/")
                },
                new DetailRelation()
                {
                    DetailRelationId = 9,
                    DetailId = 8,
                    Count = 1,
                    HierarchyId = HierarchyId.Parse("/1/5/8/")
                },
                new DetailRelation()
                {
                    DetailRelationId = 10,
                    DetailId = 9,
                    Count = 4,
                    HierarchyId = HierarchyId.Parse("/1/9/")
                }
            );

            #endregion TestData
        }
    }
}
