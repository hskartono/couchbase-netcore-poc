using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AppCoreApi.ApplicationCore.Entities;
using System;


namespace AppCoreApi.Infrastructure.Configurations
{
	public class SchedulerConfigurationConfiguration : IEntityTypeConfiguration<SchedulerConfiguration>
	{
		public void Configure(EntityTypeBuilder<SchedulerConfiguration> builder)
		{
			builder
				.HasKey(e => e.Id);
			builder
				.Property(e => e.Id)
				.UseIdentityColumn();



			builder
				.HasOne(e => e.IntervalType)
				.WithMany()
				.HasForeignKey(d => d.IntervalTypeId);
			builder
				.HasOne(e => e.JobType)
				.WithMany()
				.HasForeignKey(d => d.JobTypeId);




			builder
				.HasQueryFilter(m => EF.Property<DateTime?>(m, "DeletedAt") == null);
		}
	}
}
