using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Routing;
using Shevastream.Services;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Shevastream.Extensions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using CommonMark;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Serialization;
using Shevastream.Models;
using Shevastream.Services.Factories;
using Shevastream.ActionFilters;
using System.Linq;

namespace Shevastream
{
	public class Startup
	{
		private readonly IHostingEnvironment _env;

		public Startup(IHostingEnvironment env)
		{
			// Set up configuration sources.

			_env = env;

			var builder = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json")
				.AddJsonFile($"appsettings.{env.EnvironmentName.ToLower()}.json", optional: true)
				.AddJsonFile("version.json", optional: true)
				.AddEnvironmentVariables();

			Configuration = builder.Build();

			CurrentEnvironment = env;
		}

		public IConfigurationRoot Configuration { get; set; }
		private IHostingEnvironment CurrentEnvironment { get; set; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			// Use Entity Framework
			if (CurrentEnvironment.IsProduction())
			{
				services
					.AddEntityFrameworkNpgsql()
					.AddDbContext<DataContext>(
						b => b.UseNpgsql(Configuration["Data:ConnectionString"])
					);
			}
			else if (CurrentEnvironment.IsDevelopment())
			{
				services
					.AddEntityFrameworkSqlite()
					.AddDbContext<DataContext>(
						b => b.UseSqlite("Data Source=development.db")
					);
			}
			else
			{
				services
					.AddDbContext<DataContext>(
						b => b
							.UseInMemoryDatabase()
							.UseInternalServiceProvider(
								new ServiceCollection()
									.AddEntityFrameworkInMemoryDatabase()
									.BuildServiceProvider()
							)
					);
			}

			services.AddMvc().AddJsonOptions(opt =>
			{
				var resolver = opt.SerializerSettings.ContractResolver;
				if (resolver != null)
				{
					var res = resolver as DefaultContractResolver;
					res.NamingStrategy = null;  // <<!-- this removes the camelcasing
				}
			});

			services.AddRouting(options => { options.LowercaseUrls = true; });

			// Add application services.
			services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

			if (!_env.IsTesting())
			{
				services.AddSingleton<IHttpClientFactory, HttpClientFactory>();
			}

			services.AddSingleton<IConfiguration>(Configuration);

			services.AddTransient<IDataContext, DataContext>();
			services.AddTransient<ICryptoService, CryptoService>();
			services.AddTransient<IPushService, PushService>();
			services.AddTransient<IBlogService, BlogService>();
			services.AddTransient<IAuthService, AuthService>();
			services.AddTransient<ITransliterationService, TransliterationService>();
			services.AddTransient<ICartService, CartService>();
			services.AddTransient<IOrderService, OrderService>();
			services.AddTransient<IDataSeedService, DataSeedService>();
			services.AddTransient<ISiteMapService, SiteMapService>();
			services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

			services.AddTransient<ReCaptcha>();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IServiceProvider serviceProvider)
		{
			loggerFactory
				.AddShevastreamLogger(
					serviceProvider.GetService<IPushService>(),
					env.IsTesting() ? LogLevel.Error : Configuration["Logging:MinLogLevel"].ToEnum<LogLevel>(),
					Configuration.StringsFromArray("Logging:Exclude").ToArray()
				);

			app.UseToLowercase();

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseDatabaseErrorPage();
			}
			else
			{
				app.UseExceptionHandler("/Error");
			}

			app.UseStatusCodePagesWithReExecute("/Error/{0}");

			app.UseStaticFiles();

			// To configure external authentication please see http://go.microsoft.com/fwlink/?LinkID=532715

			var options = new CookieAuthenticationOptions();
			options.AuthenticationScheme = "MyCookieMiddlewareInstance";
			options.LoginPath = PathString.Empty; //new PathString("/Account/Denied/");
			options.AccessDeniedPath = new PathString("/Account/Login");
			options.AutomaticAuthenticate = true;
			options.AutomaticChallenge = true;
			options.CookieName = "AUTHCOOKIE";
			options.ExpireTimeSpan = new TimeSpan(1, 0, 0);
			options.CookieHttpOnly = false;

			app.UseCookieAuthentication(options);

			app.UseMvc(routes =>
			{
				routes.MapRoute(
					name: "default",
					template: "{controller=Store}/{action=Index}/{id?}"
				);
				routes.MapRoute(
					"OnlyAction",
					"{action}",
					new { controller = "Store", action = "Index" }
				);
				routes.MapRoute(
					"Error",
					"Error/{code}",
					new { controller = "Error", action = "Error" }
				);
				routes.MapRoute(
					"Blog",
					"Blog/{id}/{title?}",
					new { controller = "Blog", action = "Post" }
				);
				routes.MapRoute(
					"Sitemap",
					"sitemap.xml",
					new { controller = "Home", action = "SiteMap" }
				);

			});

			using (var context = serviceProvider.GetService<IDataContext>())
			{
				context.Database.EnsureCreated();
			}

			serviceProvider.GetRequiredService<IDataSeedService>().SeedData();
		}

		// Entry point for the application.
		public static void Main(string[] args)
		{
			var host = new WebHostBuilder()
				.UseKestrel()
				.UseContentRoot(Directory.GetCurrentDirectory())
				.UseIISIntegration()
				.UseStartup<Startup>()
				.UseUrls("http://*:5555")
				.Build();

			host.Run();
		}
	}
}
