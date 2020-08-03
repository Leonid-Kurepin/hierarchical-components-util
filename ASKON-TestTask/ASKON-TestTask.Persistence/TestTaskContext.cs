using ASKON_TestTask.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace ASKON_TestTask.Persistence
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

        private const string _connectionString =
            @"Server=.\SQLEXPRESS;Database=ASKON-TestTaskDb;Trusted_Connection=True;";

        public DbSet<Detail> Details { get; set; }
        public DbSet<DetailRelation> DetailRelations { get; set; }

        /*
        public virtual DbSet<DetailForReport> DetailsForReport { get; set; }
        */

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString,
                x => x.UseHierarchyId());
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
                    Name = "Двигатель 2106"
                },
                new Detail()
                {
                    DetailId = 2,
                    Name = "Двигатель 2103"
                },
                new Detail()
                {
                    DetailId = 3,
                    Name = "Коленвал"
                },
                new Detail()
                {
                    DetailId = 4,
                    Name = "Распредвал"
                },
                new Detail()
                {
                    DetailId = 5,
                    Name = "Поршень в сборе"
                },
                new Detail()
                {
                    DetailId = 6,
                    Name = "Поршень"
                },
                new Detail()
                {
                    DetailId = 7,
                    Name = "Компрессионное кольцо"
                },
                new Detail()
                {
                    DetailId = 8,
                    Name = "Маслосъемное кольцо"
                },
                new Detail()
                {
                    DetailId = 9,
                    Name = "Шатун"
                },
                new Detail()
                {
                    DetailId = 10,
                    Name = "Блок цилиндров"
                }
            );

            modelBuilder.Entity<DetailRelation>().HasData
            (
                new DetailRelation()
                {
                    DetailRelationId = 1,
                    DetailId = 1,
                    Count = null,
                    HierarchyLevel = HierarchyId.Parse("/1/")
                },
                new DetailRelation()
                {
                    DetailRelationId = 2,
                    DetailId = 2,
                    Count = null,
                    HierarchyLevel = HierarchyId.Parse("/2/")
                },
                new DetailRelation()
                {
                    DetailRelationId = 3,
                    DetailId = 10,
                    Count = 1,
                    HierarchyLevel = HierarchyId.Parse("/1/10/")
                },
                new DetailRelation()
                {
                    DetailRelationId = 4,
                    DetailId = 3,
                    Count = 1,
                    HierarchyLevel = HierarchyId.Parse("/1/3/")
                },
                new DetailRelation()
                {
                    DetailRelationId = 5,
                    DetailId = 4,
                    Count = 2,
                    HierarchyLevel = HierarchyId.Parse("/1/4/")
                },
                new DetailRelation()
                {
                    DetailRelationId = 6,
                    DetailId = 5,
                    Count = 4,
                    HierarchyLevel = HierarchyId.Parse("/1/5/")
                },
                new DetailRelation()
                {
                    DetailRelationId = 7,
                    DetailId = 6,
                    Count = 1,
                    HierarchyLevel = HierarchyId.Parse("/1/5/6/")
                },
                new DetailRelation()
                {
                    DetailRelationId = 8,
                    DetailId = 7,
                    Count = 2,
                    HierarchyLevel = HierarchyId.Parse("/1/5/7/")
                },
                new DetailRelation()
                {
                    DetailRelationId = 9,
                    DetailId = 8,
                    Count = 1,
                    HierarchyLevel = HierarchyId.Parse("/1/5/8/")
                },
                new DetailRelation()
                {
                    DetailRelationId = 10,
                    DetailId = 9,
                    Count = 4,
                    HierarchyLevel = HierarchyId.Parse("/1/9/")
                }
            );

            #endregion TestData
        }
    }
}
