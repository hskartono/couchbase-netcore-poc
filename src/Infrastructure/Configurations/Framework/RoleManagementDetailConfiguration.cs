using AppCoreApi.ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;


namespace AppCoreApi.Infrastructure.Configurations
{
    public class RoleManagementDetailConfiguration : IEntityTypeConfiguration<RoleManagementDetail>
    {
        public void Configure(EntityTypeBuilder<RoleManagementDetail> builder)
        {
            builder.ToTable("RoleDetails");

            builder
                .HasKey(e => e.Id);
            builder
                .Property(e => e.Id)
                .UseIdentityColumn();



            builder
                .HasOne(e => e.RoleManagement)
                .WithMany(d => d.RoleManagementDetail)
                .HasForeignKey(e => e.RoleManagementId);
            builder
                .HasOne(e => e.FunctionInfo)
                .WithMany()
                .HasForeignKey(d => d.FunctionInfoId);




            builder
                .HasQueryFilter(m => EF.Property<DateTime?>(m, "DeletedAt") == null);
        }
    }
}
