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
	public class EmailConfiguration : IEntityTypeConfiguration<Email>
	{
		public void Configure(EntityTypeBuilder<Email> builder)
		{
			builder
				.HasKey(p => p.Id);

			builder
				.HasMany(p => p.EmailAttachments)
				.WithOne(d => d.Email)
				.HasForeignKey(d => d.EmailId);

			builder
				.Property(p => p.Id)
				.UseIdentityColumn();

			builder
				.Property(p => p.Subject)
				.HasMaxLength(100);

			builder
				.Property(p => p.Sender)
				.HasMaxLength(100);

			builder
				.Property(p => p.Receiver)
				.HasMaxLength(1000);

			builder
				.Property(p => p.ReceiverCC)
				.HasMaxLength(1000);

			builder
				.Property(p => p.MailContent)
				.HasColumnType("text");

			builder
				.Property(p => p.Status)
				.HasDefaultValue(Email.EmailStatus.READY);

			builder
				.Property(p => p.RetryCount)
				.HasDefaultValue(0);

			builder
				.HasQueryFilter(m => EF.Property<DateTime?>(m, "DeletedAt") == null);
		}
	}
}
