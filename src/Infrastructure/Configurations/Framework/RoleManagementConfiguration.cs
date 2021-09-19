using AppCoreApi.ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;


namespace AppCoreApi.Infrastructure.Configurations
{
    public class RoleManagementConfiguration : IEntityTypeConfiguration<RoleManagement>
    {
        public void Configure(EntityTypeBuilder<RoleManagement> builder)
        {
            builder.ToTable("Roles");

            builder
                .HasKey(e => e.Id);
            builder
                .Property(e => e.Id)
                .UseIdentityColumn();





            builder
                .HasMany(d => d.RoleManagementDetail)
                .WithOne(p => p.RoleManagement)
                .HasForeignKey(p => p.RoleManagementId);


            builder
                .HasQueryFilter(m => EF.Property<DateTime?>(m, "DeletedAt") == null);
        }
    }
}
