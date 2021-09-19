using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AppCoreApi.ApplicationCore.Entities;
using System;

namespace AppCoreApi.Infrastructure.Configurations
{
	public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
	{
		public void Configure(EntityTypeBuilder<UserRole> builder)
		{
			builder
				.HasKey(p => p.Id);

			builder
				.Property(p => p.Id)
				.UseIdentityColumn();

			builder
				.HasOne(p => p.UserInfo)
				.WithOne();

			builder.HasOne(p => p.UserInfo)
				.WithMany()
				.HasForeignKey(p => p.UserInfoId);

			builder
				.HasMany(p => p.UserRoleDetails)
				.WithOne(d => d.UserRole)
				.HasForeignKey(d => d.UserRoleId);

			builder
				.HasQueryFilter(m => EF.Property<DateTime?>(m, "DeletedAt") == null);
		}
	}
}
