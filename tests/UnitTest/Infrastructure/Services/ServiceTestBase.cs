using AppCoreApi.ApplicationCore;
using AppCoreApi.ApplicationCore.Configurations;
using AppCoreApi.ApplicationCore.Services;
using AppCoreApi.Infrastructure;
using AppCoreApi.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Xunit;

namespace UnitTest.Infrastructure.Services
{
    public class ServiceTestBase : IClassFixture<DatabaseFixture>
	{
		protected DatabaseFixture _db;
		protected UnitOfWork _unitOfWork;
		protected IUriComposer _uriComposer;
		protected IDownloadProcessService _downloadProcessService;

		public ServiceTestBase(DatabaseFixture fixture)
		{
			_db = fixture;
			_unitOfWork = new UnitOfWork(_db.context);

			InitUriComposer();
			_downloadProcessService = new DownloadProcessService(_unitOfWork);
		}

		protected void InitUriComposer()
		{
			var lookup = new LookupSettings()
			{
				PathConfig = new PathConfig()
                {
					BaseStoragePath = "",
					BaseURI = "",
					DownloadPath = "",
					DownloadURI = "",
					LogsPath = "",
					TemplatePath = "",
					TempPath = "",
					UploadPath = "",
					UploadURI = ""
				}
			};

			_uriComposer = new UriComposer(lookup);
		}

		protected DataTable SqlToDataTable(string sql)
		{
			DataTable dt = new();
			var conn = _db.context.Database.GetDbConnection();
			if (conn.State != ConnectionState.Open)
				conn.Open();
			using (var cmd = conn.CreateCommand())
			{
				cmd.CommandText = sql;
				cmd.CommandType = CommandType.Text;
				var reader = cmd.ExecuteReader();
				dt.Load(reader);
				reader.Close();
			}
			conn.Close();

			return dt;
		}

		protected int ExecuteNonQuery(string sql)
		{
			int result = 0;
			var conn = _db.context.Database.GetDbConnection();
			if(conn.State != ConnectionState.Open)
				conn.Open();
			using (var cmd = conn.CreateCommand())
			{
				cmd.CommandText = sql;
				cmd.CommandType = CommandType.Text;
				result = cmd.ExecuteNonQuery();
			}
			conn.Close();

			return result;
		}

		// data preparation
		
	}
}
