using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AppCoreApi.ApplicationCore.Entities;
using System;

namespace AppCoreApi.Infrastructure.Configurations
{
	public class FunctionInfoConfiguration : IEntityTypeConfiguration<FunctionInfo>
	{
		public void Configure(EntityTypeBuilder<FunctionInfo> builder)
		{
			builder
				.HasKey(p => p.Id);

			builder
				.Property(p => p.Name)
				.HasMaxLength(100);

			builder
				.Property(p => p.IconName)
				.HasMaxLength(100);

			builder
				.Property(p => p.Uri)
				.HasMaxLength(100);

			builder
				.HasOne(e => e.ModuleInfo)
				.WithMany()
				.HasForeignKey(e => e.ModuleInfoId)
				.OnDelete(DeleteBehavior.Restrict);

			builder
				.HasQueryFilter(m => EF.Property<DateTime?>(m, "DeletedAt") == null);
		}
	}
}
