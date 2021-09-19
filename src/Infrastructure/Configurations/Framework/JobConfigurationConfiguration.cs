using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AppCoreApi.ApplicationCore.Entities;
using System;


namespace AppCoreApi.Infrastructure.Configurations
{
	public class JobConfigurationConfiguration : IEntityTypeConfiguration<JobConfiguration>
	{
		public void Configure(EntityTypeBuilder<JobConfiguration> builder)
		{
			builder
				.HasKey(e => e.Id);
			builder
				.Property(e => e.Id)
				.UseIdentityColumn();







			builder
				.HasQueryFilter(m => EF.Property<DateTime?>(m, "DeletedAt") == null);
		}
	}
}
