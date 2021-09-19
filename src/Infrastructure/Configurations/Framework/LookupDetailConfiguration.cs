using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AppCoreApi.ApplicationCore.Entities;
using System;

namespace AppCoreApi.Infrastructure.Configurations
{
	public class LookupDetailConfiguration : IEntityTypeConfiguration<LookupDetail>
	{
		public void Configure(EntityTypeBuilder<LookupDetail> builder)
		{
			builder.HasKey(e => e.Id);
			builder.Property(e => e.Id).UseIdentityColumn();
			builder.HasOne(e => e.Lookup).WithMany(d=>d.LookupDetails).HasForeignKey(e => e.LookupId);
			builder
				.HasQueryFilter(m => EF.Property<DateTime?>(m, "DeletedAt") == null);
		}
	}
}
