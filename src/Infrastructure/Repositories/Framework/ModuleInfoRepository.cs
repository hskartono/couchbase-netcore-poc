using Microsoft.EntityFrameworkCore;
using AppCoreApi.ApplicationCore.Entities;
using AppCoreApi.ApplicationCore.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AppCoreApi.Infrastructure.Repositories
{
	public class ModuleInfoRepository : AsyncRepository<ModuleInfo>, IModuleInfoRepository
	{
		#region appgen: private variable
		public ModuleInfoRepository(AppDbContext context) : base(context) { }
		#endregion

		#region appgen: constructor
		private AppDbContext MyDbContext
		{
			get { return Context as AppDbContext; }
		}
		#endregion

		#region appgen: generated methods
		public async Task<ModuleInfo> CloneEntity(int id, string userName)
		{
			var entity = await MyDbContext.Set<ModuleInfo>().AsQueryable()
				.Where(e => e.Id == id)
				.AsNoTracking()
				.SingleOrDefaultAsync();
			entity.Id = 0;
			entity.IsDraftRecord = (int)BaseEntity.DraftStatus.DraftMode;
			entity.MainRecordId = id;
			entity.RecordActionDate = DateTime.Now;
			entity.RecordEditedBy = userName;



			await MyDbContext.AddAsync(entity);
			await MyDbContext.SaveChangesAsync();
			return entity;
		}
		#endregion

		#region Tambahan REZA
		public async Task<bool> searcFungsionInfos(int id)
		{
			var sql1 = $@"select FI.ModuleInfoId from FunctionInfos as FI where 
						FI.DeletedAt is null 
						and FI.IsDraftRecord = 0 
						and Coalesce(FI.DraftFromUpload,0) = 0
						and FI.ModuleInfoId = {id}";

			var data = await SqlToDataTableAsync(sql1);

			if (data != null && data.Rows.Count > 0)
			{
				return false;
			}
			else
			{
				return true;
			}
		}
		#endregion
	}
}
