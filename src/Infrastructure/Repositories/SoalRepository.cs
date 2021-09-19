using AppCoreApi.ApplicationCore.Entities;
using AppCoreApi.ApplicationCore.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace AppCoreApi.Infrastructure.Repositories
{
	public class SoalRepository : AsyncCouchbaseRepository<Soal>, ISoalRepository
	{
		#region appgen: private variable
		public SoalRepository(ICouchbaseRepository context) : base(context) { }
		#endregion

		#region appgen: constructor
		//private AppDbContext MyDbContext
		//{
		//	get { return Context as AppDbContext; }
		//}
		#endregion

		#region appgen: generated methods
		//public async Task<Soal> CloneEntity(string id, string userName)
		//{
		//	var entity = await MyDbContext.Set<Soal>().AsQueryable()
		//		.Where(e => e.Id == id)
		//		.AsNoTracking()
		//		.SingleOrDefaultAsync();
		//	entity.Id = "";
		//	entity.IsDraftRecord = (int)BaseEntity.DraftStatus.DraftMode;
		//	entity.MainRecordId = id;
		//	entity.RecordActionDate = DateTime.Now;
		//	entity.RecordEditedBy = userName;



		//	await MyDbContext.AddAsync(entity);
		//	await MyDbContext.SaveChangesAsync();
		//	return entity;
		//}
		#endregion
	}
}
