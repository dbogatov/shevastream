using System;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using EShop.Services;
using Microsoft.AspNet.Http;

namespace EShop
{
	public class Startup
	{
		public Startup(IHostingEnvironment env)
		{
			// Set up configuration sources.

			var builder = new ConfigurationBuilder()
				.AddJsonFile("appsettings.json")
				.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

			if (env.IsDevelopment())
			{
				// For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
				builder.AddUserSecrets();

				// This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
				builder.AddApplicationInsightsSettings(developerMode: true);
			}

			builder.AddEnvironmentVariables();
			Configuration = builder.Build();
		}

		public IConfigurationRoot Configuration { get; set; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			// Add framework services.
			services.AddApplicationInsightsTelemetry(Configuration);

			services.AddEntityFramework()
				.AddNpgsql()
				.AddDbContext<DataContext>();

			DataContext.connectionString = Configuration["Data:PGSQLConnection:ConnectionString"]; 

			services.AddMvc();

			// Add application services.
			services.AddTransient<ICryptoService, CryptoService>();

			services.AddTransient<DataContext, DataContext>();

			services.AddTransient<ITelegramSender, TelegramSender>();
			services.AddTransient<IDBLogService, DBLogService>();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IServiceProvider serviceProvider)
		{
			loggerFactory.AddConsole(Configuration.GetSection("Logging"));
			loggerFactory.AddDebug();

			app.UseApplicationInsightsRequestTelemetry();

			if (env.IsDevelopment())
			{
				app.UseBrowserLink();
				app.UseDeveloperExceptionPage();
				app.UseDatabaseErrorPage();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
			}

			app.UseIISPlatformHandler(options => options.AuthenticationDescriptions.Clear());

			app.UseApplicationInsightsExceptionTelemetry();

			app.UseStaticFiles();

			// To configure external authentication please see http://go.microsoft.com/fwlink/?LinkID=532715

			app.UseCookieAuthentication(options =>
			{
				options.AuthenticationScheme = "MyCookieMiddlewareInstance";
				options.LoginPath = PathString.Empty; //new PathString("/Account/Unauthorized/");
				options.AccessDeniedPath = new PathString("/Account/Forbidden/");
				options.AutomaticAuthenticate = true;
				options.AutomaticChallenge = true;
				options.CookieName = "AUTHCOOKIE";
				options.ExpireTimeSpan = new TimeSpan(1, 0, 0);
				options.CookieHttpOnly = false;
			});

			app.UseMvc(routes =>
			{
				routes.MapRoute(
					name: "default",
					template: "{controller=Home}/{action=Index}/{id?}");
				routes.MapRoute(
					"OnlyAction",
					"{action}",
					new { controller = "Home", action = "Index" }
				);
			});

			using (var context = serviceProvider.GetService<DataContext>())
			{
				context.Database.EnsureCreated();
				context.EnsureSeedData();
			}
		}

		// Entry point for the application.
		public static void Main(string[] args) => WebApplication.Run<Startup>(args);
	}
}
