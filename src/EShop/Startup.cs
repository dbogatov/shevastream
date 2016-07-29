using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Routing;
using EShop.Services;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using EShop.Extensions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using CommonMark;
using EShop.Models;
using Newtonsoft.Json.Serialization;
using WebMarkupMin.AspNetCore1;

namespace EShop
{
	public class Startup
	{
		public Startup(IHostingEnvironment env)
		{
			// Set up configuration sources.

			var builder = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json")
				.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

			if (env.IsDevelopment())
			{
				// For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
				builder.AddUserSecrets();
			}

			//builder.AddEnvironmentVariables();
			Configuration = builder.Build();
		}

		public IConfigurationRoot Configuration { get; set; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services
				.AddEntityFrameworkNpgsql()
				.AddDbContext<DataContext>();

			DataContext.connectionString = Configuration["Data:PGSQLConnection:ConnectionString"];

			// Add WebMarkupMin services.
			// services
			// 	.AddWebMarkupMin(options => {
			// 		options.AllowMinificationInDevelopmentEnvironment = true;
			// 		options.AllowCompressionInDevelopmentEnvironment = true;
			// 	})
			// 	.AddHtmlMinification()
			// 	.AddXmlMinification()
			// 	.AddHttpCompression();

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
			services.AddTransient<ITelegramSender, TelegramSender>();
			services.AddTransient<IDBLogService, DBLogService>();
			services.AddTransient<IPushService, PushService>();
			services.AddTransient<IBlogService, BlogService>();
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
					template: "{controller=Store}/{action=Index}/{id?}");
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
					"Blog/{id}/{title}",
					new { controller = "Blog", action = "Post" }
				);
				routes.MapRoute(
					"Sitemap",
					"sitemap.xml",
					new { controller = "Store", action = "SiteMap" }
				);

			});

			using (var context = serviceProvider.GetService<DataContext>())
			{
				context.Database.EnsureCreated();
			}

			serviceProvider.GetService<IDataSeedService>().SeedData();

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
				.UseUrls("http://localhost:5000")
				.Build();

			host.Run();
		}
	}
}
