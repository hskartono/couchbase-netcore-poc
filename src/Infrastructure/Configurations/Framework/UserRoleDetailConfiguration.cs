using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AppCoreApi.ApplicationCore.Entities;
using System;

namespace AppCoreApi.Infrastructure.Configurations
{
	public class UserRoleDetailConfiguration : IEntityTypeConfiguration<UserRoleDetail>
	{
		public void Configure(EntityTypeBuilder<UserRoleDetail> builder)
		{
			builder.HasKey(p => p.Id);
			builder
				.Property(urd => urd.Id)
				.UseIdentityColumn();

			builder
				.Property(urd => urd.RoleId)
				.IsRequired();

			builder
				.Property(urd => urd.UserRoleId)
				.IsRequired();

			builder.HasOne(urd => urd.Role)
				.WithMany()
				.HasForeignKey(urd => urd.RoleId);

			builder.HasOne(urd => urd.UserRole)
				.WithMany(ur => ur.UserRoleDetails)
				.HasForeignKey(urd => urd.UserRoleId);

			builder
				.HasQueryFilter(m => EF.Property<DateTime?>(m, "DeletedAt") == null);
		}
	}
}
