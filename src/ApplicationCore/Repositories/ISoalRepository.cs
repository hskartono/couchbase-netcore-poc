using AppCoreApi.ApplicationCore.Entities;
using System.Threading.Tasks;

namespace AppCoreApi.ApplicationCore.Repositories
{
    public interface ISoalRepository : IAsyncRepository<Soal>
	{
		#region appgen: repository method
		//Task<Soal> CloneEntity(string id, string userName);

		#endregion
	}
}
