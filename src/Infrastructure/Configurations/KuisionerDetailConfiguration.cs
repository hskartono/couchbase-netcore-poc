using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AppCoreApi.ApplicationCore.Entities;
using System;


namespace AppCoreApi.Infrastructure.Configurations
{
	public class KuisionerDetailConfiguration : IEntityTypeConfiguration<KuisionerDetail>
	{
		public void Configure(EntityTypeBuilder<KuisionerDetail> builder)
		{
			builder
				.HasKey(e => e.Id);
			builder
				.Property(e => e.Id)
				.UseIdentityColumn();



			builder
				.HasOne(e => e.Kuisioner)
				.WithMany(d => d.KuisionerDetail)
				.HasForeignKey(e => e.KuisionerId);
			builder
				.HasOne(e => e.Soal)
				.WithMany()
				.HasForeignKey(d => d.SoalId);




			builder
				.HasQueryFilter(m => EF.Property<DateTime?>(m, "DeletedAt") == null);
		}
	}
}
