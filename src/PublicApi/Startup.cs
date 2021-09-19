using Audit.Core;
using Audit.WebApi;
using AutoMapper;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using AppCoreApi.ApplicationCore;
using AppCoreApi.ApplicationCore.Services;
using AppCoreApi.Infrastructure;
using AppCoreApi.Infrastructure.Services;
using AppCoreApi.PublicApi;
using AppCoreApi.PublicApi.Filters;
using System;
using System.IO;
using System.Security.Claims;
using System.Text;
using AppCoreApi.Infrastructure.Repositories;
using AppCoreApi.ApplicationCore.Repositories;

namespace PublicApi
{
	public class Startup
	{
		//readonly string debugOrigin = "_debugOrigins";

		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			#region CORS
			//services.AddCors(options =>
			//{
			//	options.AddPolicy(name: debugOrigin,
			//		builder =>
			//		{
			//			builder.WithOrigins("http://localhost:44313/",
			//					  "http://localhost:8080/");
			//		});
			//});
			#endregion

			#region Configuration
			services.Configure<LookupSettings>(Configuration);
			services.AddSingleton<IUriComposer>(new UriComposer(Configuration.Get<LookupSettings>()));
			services.AddSingleton<IAppSettings>(new AppSettings(Configuration.Get<LookupSettings>()));
            #endregion

            #region Audit Trail
            // audit trail configuration
            services.AddControllers(o =>
            {
                o.Filters.Add(new ApiExceptionFilterAttribute());
                o.Filters.Add(new LoginFilter());
                //o.AddAuditFilter(j => j
                //    .LogAllActions()
                //    .IncludeHeaders(ctx => !ctx.ModelState.IsValid)
                //    .IncludeRequestBody()
                //    .IncludeModelState()
                //    .IncludeResponseBody(ctx => ctx.HttpContext.Response.StatusCode == 200)
                //);
            });

			// audit trail save to database
			//Audit.Core.Configuration.Setup()
			//	.UseSqlServer(config => config
			//		.ConnectionString(Configuration.GetConnectionString("AuditTrail"))
			//		.Schema("dbo")
			//		.TableName("Event")
			//		.IdColumnName("EventId")
			//		.JsonColumnName("JsonData")
			//		.LastUpdatedColumnName("LastUpdatedDate")
			//		.CustomColumn("EventType", ev => ev.EventType)
			//		.CustomColumn("User", ev => ev.Environment.UserName));
			#endregion

			#region Framework Services
			var couchbaseConfig = Configuration.GetSection("couchbase");
			services.AddSingleton<ICouchbaseRepository>(new CouchbaseRepository(couchbaseConfig));
			services.AddScoped<IUnitOfWork, UnitOfWork>();
			services.AddTransient<IAttachmentService, AttachmentService>();
			services.AddTransient<ICompanyService, CompanyService>();
			services.AddTransient<IEmailService, EmailService>();
			services.AddTransient<IFunctionInfoService, FunctionInfoService>();
			services.AddTransient<IRoleService, RoleService>();
			services.AddTransient<IUserInfoService, UserInfoService>();
			services.AddTransient<IUserRoleService, UserRoleService>();
			services.AddTransient<ISchedulerCronIntervalService, SchedulerCronIntervalService>();
			services.AddTransient<IJobConfigurationService, JobConfigurationService>();
			services.AddTransient<ISchedulerConfigurationService, SchedulerConfigurationService>();
			services.AddTransient<IDownloadProcessService, DownloadProcessService>();
			services.AddTransient<ILookupService, LookupService>();
			services.AddTransient<ILookupDetailService, LookupDetailService>();
			services.AddTransient<ISchedulerCronIntervalService, SchedulerCronIntervalService>();
			services.AddTransient<IJobConfigurationService, JobConfigurationService>();
			services.AddTransient<ISchedulerConfigurationService, SchedulerConfigurationService>();
			services.AddTransient<IModuleInfoService, ModuleInfoService>();
			services.AddTransient<IRoleManagementService, RoleManagementService>();
			services.AddTransient<IRoleManagementDetailService, RoleManagementDetailService>();
			#endregion

			// do not remove region marker. this marker is used by code generator
			#region Application Service Configuration

			services.AddTransient<ISoalService, SoalService>();
			services.AddTransient<IKuisionerService, KuisionerService>();
			services.AddTransient<IKuisionerDetailService, KuisionerDetailService>();
			#endregion

			#region Hangfire
			// hangfire
			services.AddHangfire(opt => opt
				.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
				.UseSimpleAssemblyNameTypeSerializer()
				.UseRecommendedSerializerSettings()
				.UseSqlServerStorage(Configuration.GetConnectionString("HangfireDb"), new SqlServerStorageOptions()
				{
					CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
					SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
					QueuePollInterval = TimeSpan.Zero,
					UseRecommendedIsolationLevel = true,
					DisableGlobalLocks = true
				}));
			#endregion

			#region Database Connection
			// database connection
			services.AddDbContext<AppDbContext>(
				o => o.UseSqlServer(
						Configuration.GetConnectionString("Default"),
						x => x.MigrationsAssembly("Infrastructure")
					)
				);
			#endregion

			#region Swagger
			// swagger
			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new OpenApiInfo { Title = "PublicApi", Version = "v1" });
			});
			#endregion

			#region Automapper
			// automapper
			services.AddAutoMapper(typeof(Startup));
			#endregion

			#region JWT Authentication
			// 1. Add Authentication Services
			services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			}).AddJwtBearer(options =>
			{
				options.RequireHttpsMetadata = false;
				options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
				{
					ValidateIssuer = true,
					ValidateAudience = true,
					ValidIssuer = "TAM.Passport",
					ValidAudience = Configuration.GetSection("Passport:clientId").Value,
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.GetSection("Passport:clientSecret").Value))
				};
			});
			#endregion

			#region Json serializer loop handling
			services.AddControllers().AddNewtonsoftJson(x => x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
			#endregion
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHttpContextAccessor ctxAccessor)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			#region body context rewinder
			// enable context rewinder (karena sudah diakses oleh log)
			//app.Use(async (context, next) => {  // <----
			//	context.Request.EnableBuffering(); // or .EnableRewind();
			//	await next();
			//});
			#endregion

			#region Swagger
			// swagger
			app.UseSwagger();
			app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PublicApi v1"));
			#endregion

			#region CORS, Authentication & Authorization, Routing 
			app.UseHttpsRedirection();

            // global cors policy
            app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            //app.UseCors(debugOrigin);

            // 2. Enable authentication middleware
            app.UseAuthentication();
			app.UseRouting();
			app.UseAuthorization();
			#endregion

			#region Audit Trail
			// audit trail
			//Audit.Core.Configuration.AddCustomAction(ActionType.OnScopeCreated, scope =>
			//{
			//	scope.Event.Environment.UserName = ctxAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
			//});
			#endregion

			#region Static file serve
			// bind static file for download
			app.UseStaticFiles(new StaticFileOptions
			{
				FileProvider = new PhysicalFileProvider(
					 Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Storage", "Downloads")),
				RequestPath = "/api/v1/DownloadFiles"
			});

			// bind static file for uploaded files
			app.UseStaticFiles(new StaticFileOptions
			{
				FileProvider = new PhysicalFileProvider(
					 Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Storage", "Uploads")),
				RequestPath = "/api/v1/UploadedFiles"
			});

			// bind static file for resource file
			app.UseStaticFiles(new StaticFileOptions
			{
				FileProvider = new PhysicalFileProvider(
					 Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Storage", "res")),
				RequestPath = "/res"
			});
			#endregion

			#region Hangfire dashboard security & server initialization
			// hangfire dashboard
			app.UseHangfireDashboard("/hangfire", new DashboardOptions() { 
				Authorization = new[] { 
					new NoSecurityAuthorizationFilter() 
				} 
			});

			// hangfire server
			app.UseHangfireServer();
			#endregion

			#region Endpoint & hangfire dashboard
			// end point
			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
				endpoints.MapHangfireDashboard();
			});
			#endregion
		}
	}
}
