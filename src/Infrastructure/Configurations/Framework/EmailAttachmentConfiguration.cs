using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AppCoreApi.ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCoreApi.Infrastructure.Configurations
{
	public class EmailAttachmentConfiguration : IEntityTypeConfiguration<EmailAttachment>
	{
		public void Configure(EntityTypeBuilder<EmailAttachment> builder)
		{
			builder
				.HasKey(p => p.Id);

			builder
				.HasOne(p => p.Email)
				.WithMany(d => d.EmailAttachments)
				.HasForeignKey(p => p.EmailId);

			builder
				.Property(p => p.Id)
				.UseIdentityColumn();

			builder
				.HasQueryFilter(m => EF.Property<DateTime?>(m, "DeletedAt") == null);
		}
	}
}
