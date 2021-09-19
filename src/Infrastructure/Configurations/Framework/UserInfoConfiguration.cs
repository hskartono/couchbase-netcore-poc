using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AppCoreApi.ApplicationCore.Entities;
using System;

namespace AppCoreApi.Infrastructure.Configurations
{
	public class UserInfoConfiguration : IEntityTypeConfiguration<UserInfo>
	{
		public void Configure(EntityTypeBuilder<UserInfo> builder)
		{
			builder
				.HasKey(p => p.UserName);

			builder
				.Property(p => p.UserName)
				.HasMaxLength(100);

			builder
				.Property(p => p.FirstName)
				.HasMaxLength(100);

			builder
				.Property(p => p.LastName)
				.HasMaxLength(100);
			builder
				.HasQueryFilter(m => EF.Property<DateTime?>(m, "DeletedAt") == null);
		}
	}
}
