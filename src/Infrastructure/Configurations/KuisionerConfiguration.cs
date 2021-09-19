using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AppCoreApi.ApplicationCore.Entities;
using System;


namespace AppCoreApi.Infrastructure.Configurations
{
	public class KuisionerConfiguration : IEntityTypeConfiguration<Kuisioner>
	{
		public void Configure(EntityTypeBuilder<Kuisioner> builder)
		{
			builder
				.HasKey(e => e.Id);
			builder
				.Property(e => e.Id)
				.UseIdentityColumn();





			builder
				.HasMany(d => d.KuisionerDetail)
				.WithOne(p => p.Kuisioner)
				.HasForeignKey(p => p.KuisionerId);


			builder
				.HasQueryFilter(m => EF.Property<DateTime?>(m, "DeletedAt") == null);
		}
	}
}
