using Microsoft.EntityFrameworkCore;
using AppCoreApi.ApplicationCore.Entities;
using AppCoreApi.ApplicationCore.Repositories;
using AppCoreApi.ApplicationCore.Specifications.Filters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AppCoreApi.Infrastructure.Repositories
{
	public class KuisionerRepository : AsyncRepository<Kuisioner>, IKuisionerRepository
	{
		#region appgen: private variable
		public KuisionerRepository(AppDbContext context) : base(context) { }
		#endregion

		#region appgen: constructor
		private AppDbContext MyDbContext
		{
			get { return Context as AppDbContext; }
		}
		#endregion

		#region appgen: generated methods
		public async Task<Kuisioner> CloneEntity(int id, string userName)
		{
			var entity = await MyDbContext.Set<Kuisioner>().AsQueryable()
				.Where(e => e.Id == id)
				.AsNoTracking()
				.SingleOrDefaultAsync();
			entity.Id = 0;
			entity.IsDraftRecord = (int)BaseEntity.DraftStatus.DraftMode;
			entity.MainRecordId = id;
			entity.RecordActionDate = DateTime.Now;
			entity.RecordEditedBy = userName;

			var kuisionerDetails = await MyDbContext.Set<KuisionerDetail>().AsQueryable()
				.Where(e => e.Kuisioner.Id == id)
				.AsNoTracking()
				.ToListAsync();
			entity.ClearKuisionerDetails();
			entity.AddRangeKuisionerDetails(kuisionerDetails
				.Select(e => {
					e.IsDraftRecord = (int)BaseEntity.DraftStatus.DraftMode;
					e.MainRecordId = e.Id;
					e.RecordActionDate = DateTime.Now;
					e.RecordEditedBy = userName;
					e.Id = 0;
					return e;
				})
				.ToList());


			await MyDbContext.AddAsync(entity);
			await MyDbContext.SaveChangesAsync();
			return entity;
		}
		#endregion
	}
}
