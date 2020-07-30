﻿using ASKON_TestTask.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ASKON_TestTask.Persistence.EntitiesConfigurations
{
    public class DetailConfiguration : IEntityTypeConfiguration<Detail>
    {
        public void Configure(EntityTypeBuilder<Detail> builder)
        {
            // DetailId
            builder
                .Property(d => d.DetailId)
                .UseIdentityColumn();

            // Name
            builder
                .Property(d => d.Name)
                .HasMaxLength(255)
                .IsRequired();
        }
    }
}
