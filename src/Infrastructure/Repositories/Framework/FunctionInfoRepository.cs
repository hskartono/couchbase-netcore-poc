using AppCoreApi.ApplicationCore.Entities;
using AppCoreApi.ApplicationCore.Repositories;
using System.Threading.Tasks;

namespace AppCoreApi.Infrastructure.Repositories
{
	public class FunctionInfoRepository : AsyncRepository<FunctionInfo>, IFunctionInfoRepository
	{
		public FunctionInfoRepository(AppDbContext context) : base(context) { }

		public async Task<bool> searcFungsionInfos(string id)
		{
			var sql1 = $@"select FI.FunctionInfoId from RoleDetails as FI where 
						FI.DeletedAt is null 
						and FI.IsDraftRecord = 0 
						and Coalesce(FI.DraftFromUpload,0) = 0
						and FI.FunctionInfoId = '{id}'";

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


	}
}
