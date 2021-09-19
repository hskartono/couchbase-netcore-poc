using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using AppCoreApi.Infrastructure;
using PublicApi;
using System.Net.Http;
using WebMotions.Fake.Authentication.JwtBearer;

namespace UnitTest.Api
{
	public class WebApiTesterFixture
	{
		protected TestServer testServer;
		protected HttpClient client;
		protected AppDbContext _context;

		public WebApiTesterFixture()
		{
			testServer = new TestServer(new WebHostBuilder()
				.UseStartup<Startup>()
				.ConfigureTestServices(services =>
				{
					services.RemoveAll(typeof(DbContextOptions<AppDbContext>));
					services.AddDbContext<AppDbContext>(options => { options.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=ppos;Integrated Security=True;"); });
					services.AddHangfire(opt => opt.UseMemoryStorage());

					// set authentication ke fake jwt
					services.AddAuthentication(options =>
					{
						options.DefaultAuthenticateScheme = FakeJwtBearerDefaults.AuthenticationScheme;
						options.DefaultChallengeScheme = FakeJwtBearerDefaults.AuthenticationScheme;
					}).AddFakeJwtBearer();

					services.BuildServiceProvider();
				})
			);

			_context = testServer.Host.Services.CreateScope().ServiceProvider.GetRequiredService<AppDbContext>();
			_context.Database.EnsureDeleted();
			_context.Database.EnsureCreated();

			client = testServer.CreateClient();
		}

		public AppDbContext Context
		{
			get
			{
				return _context;
			}
		}

		public TestServer TestServer { 
			get { 
				return testServer; 
			} 
		}

		public HttpClient Client { 
			get { 
				return client; 
			} 
		}
	}
}
