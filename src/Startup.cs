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

namespace Shevastream
{
	public class Startup
	{
		public Startup(IHostingEnvironment env)
		{
			// Set up configuration sources.

			var builder = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json")
				.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
				.AddJsonFile("version.json", optional: true);

			//builder.AddEnvironmentVariables();
			Configuration = builder.Build();
		}

		public IConfigurationRoot Configuration { get; set; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
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

			DataContext.connectionString = Configuration["Data:PGSQLConnection:ConnectionString"];
			DataContext.version = "Lol"; //Configuration["Version:GitHash"];

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

			services.AddTransient<DataContext, DataContext>();

			services.AddTransient<ICryptoService, CryptoService>();
			services.AddTransient<IPushService, PushService>();
			services.AddTransient<IBlogService, BlogService>();
			services.AddTransient<ITransliterationService, TransliterationService>();
			services.AddTransient<ICartService, CartService>();
			services.AddTransient<IOrderService, OrderService>();
			services.AddTransient<IDataSeedService, DataSeedService>();
			services.AddTransient<ISiteMapService, SiteMapService>();
			services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IServiceProvider serviceProvider)
		{
			loggerFactory.AddConsole(Configuration.GetSection("Logging"));
			loggerFactory.AddDebug();

			app.UseToLowercase();

			// app.UseWebMarkupMin();

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
			//app.UseStatusCodePages();

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

			using (var context = serviceProvider.GetService<DataContext>())
			{
				context.Database.EnsureCreated();
			}

			// Seed the database. Should be awaited but not possible now
			if (env.IsTesting())
			{
				// Testing requires synchronous code
				// Test runner (at least XUnit) tends to run tests in parallel
				// When 2+ threads try to setup a virtual server in an async environment,
				// deadlock usually happens.
				serviceProvider.GetRequiredService<IDataSeedService>().SeedData();
			}
			else
			{
				serviceProvider.GetRequiredService<IDataSeedService>().SeedDataAsync().Wait();
			}

			// set the default HTML formatter for all future conversions
			CommonMarkSettings.Default.OutputDelegate =
				(doc, output, settings) =>
				new MyFormatter(output, settings).WriteDocument(doc);
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
