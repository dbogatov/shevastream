﻿using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using EShop.Services;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.Extensions.DependencyInjection;

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

			services.AddMvc();

			// Add application services.
			services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
			
			services.AddTransient<ICryptoService, CryptoService>();

			services.AddTransient<DataContext, DataContext>();

			services.AddTransient<ITelegramSender, TelegramSender>();
			services.AddTransient<IDBLogService, DBLogService>();
			services.AddTransient<IPushService, PushService>();
            services.AddTransient<IBlogService, BlogService>();
			services.AddTransient<ICartService, CartService>();
        }

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IServiceProvider serviceProvider)
		{
			loggerFactory.AddConsole(Configuration.GetSection("Logging"));
			loggerFactory.AddDebug();

			if (env.IsDevelopment())
			{
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
				app.UseRuntimeInfoPage();
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
					"Blog/{title}",
					new { controller = "Blog", action = "Post" }
				);
				
            });

			using(var context = serviceProvider.GetService<DataContext>())
			{
				context.Database.EnsureCreated();
				context.EnsureSeedData();
			}
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
