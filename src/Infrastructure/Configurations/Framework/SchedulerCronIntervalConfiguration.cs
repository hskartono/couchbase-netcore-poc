using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AppCoreApi.ApplicationCore.Entities;
using System;


namespace AppCoreApi.Infrastructure.Configurations
{
	public class SchedulerCronIntervalConfiguration : IEntityTypeConfiguration<SchedulerCronInterval>
	{
		public void Configure(EntityTypeBuilder<SchedulerCronInterval> builder)
		{
			builder
				.HasKey(e => e.Id);






			builder
				.HasQueryFilter(m => EF.Property<DateTime?>(m, "DeletedAt") == null);
		}
	}
}
