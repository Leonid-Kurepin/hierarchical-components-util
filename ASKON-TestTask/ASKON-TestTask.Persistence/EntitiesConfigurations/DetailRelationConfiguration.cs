using ASKON_TestTask.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ASKON_TestTask.Persistence.EntitiesConfigurations
{
    class DetailRelationConfiguration : IEntityTypeConfiguration<DetailRelation>
    {
        public void Configure(EntityTypeBuilder<DetailRelation> builder)
        {
            // DetailRelationId
            builder
                .Property(dr => dr.DetailRelationId)
                .UseIdentityColumn();

            // HierarchyLevel
            builder
                .Property(dr => dr.HierarchyLevel)
                .HasColumnType("hierarchyId")
                .IsRequired();

            // HierarchyLevel index
            builder.HasIndex(dr => dr.HierarchyLevel)
                .HasName("RelationHierarchyUnique")
                .IsUnique()
                .IsClustered(false);

            // Count
            builder
                .Property(dr => dr.Count)
                .IsRequired(false);

            // Detail - DetailRelation (1:M)
            builder.HasOne<Detail>(dr => dr.Detail)
                .WithMany(d => d.DetailRelations)
                .HasForeignKey(dr => dr.DetailId);
        }
    }
}
