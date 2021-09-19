using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AppCoreApi.ApplicationCore.Entities;
using System;

namespace AppCoreApi.Infrastructure.Configurations
{
	public class AttachmentConfiguration : IEntityTypeConfiguration<Attachment>
	{
		public void Configure(EntityTypeBuilder<Attachment> builder)
		{
			builder
				.HasKey(p => p.Id);

			builder
				.Property(p => p.Id)
				.UseIdentityColumn();

			builder
				.Property(p => p.OriginalFileName)
				.HasMaxLength(100)
				.IsRequired();
			builder
				.Property(p => p.SavedFileName)
				.HasMaxLength(100);
			builder
				.Property(p => p.FileExtension)
				.HasMaxLength(100);

			builder
				.Property(p => p.SavedFileName)
				.IsRequired();
			builder
				.HasQueryFilter(m => EF.Property<DateTime?>(m, "DeletedAt") == null);
		}
	}
}
