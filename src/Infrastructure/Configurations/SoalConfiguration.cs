using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AppCoreApi.ApplicationCore.Entities;
using System;


namespace AppCoreApi.Infrastructure.Configurations
{
	public class SoalConfiguration : IEntityTypeConfiguration<Soal>
	{
		public void Configure(EntityTypeBuilder<Soal> builder)
		{
			builder
				.HasKey(e => e.Id);






			builder
				.HasQueryFilter(m => EF.Property<DateTime?>(m, "DeletedAt") == null);
		}
	}
}
