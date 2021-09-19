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
	public class SpResultNumberGeneratorConfiguration : IEntityTypeConfiguration<SpResultNumberGenerator>
	{
		public void Configure(EntityTypeBuilder<SpResultNumberGenerator> builder)
		{
			builder.HasNoKey();
		}
	}
}
