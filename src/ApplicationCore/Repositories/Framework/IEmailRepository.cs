using AppCoreApi.ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppCoreApi.ApplicationCore.Repositories
{
	public interface IEmailRepository : IAsyncRepository<Email>
	{
	}
}
