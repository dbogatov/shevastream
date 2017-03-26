using Microsoft.Extensions.DependencyInjection;
using Shevastream.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Shevastream.Models;
using System;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Http;
using Shevastream.Services.Factories;
using Shevastream.Tests.Mock;

namespace Shevastream.Tests
{
	/// <summary>
	/// Utility class for testing methods
	/// </summary>
	static class Extensions
	{
		/// <summary>
		/// Registers all available services for testing environment.
		/// </summary>
		/// <returns>Service provider with all services registered</returns>
		public static IServiceCollection RegisterServices(IServiceCollection givenServices = null)
		{
			// Needed for DataContext
			var efServiceProvider = new ServiceCollection().AddEntityFrameworkInMemoryDatabase().BuildServiceProvider();

			var services = givenServices ?? new ServiceCollection();

			// Create service of DataContext with inMemory data provider
			services
				.AddDbContext<DataContext>(
					b => b
						.UseInMemoryDatabase()
						.UseInternalServiceProvider(efServiceProvider)
				);

			services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
			services.AddSingleton<IHttpClientFactory, HttpClientFactory>();

			services.AddTransient<IDataSeedService, DataSeedService>();
			
			services.AddTransient<ICryptoService, CryptoService>();
			services.AddTransient<IPushService, PushService>();
			services.AddTransient<IBlogService, BlogService>();
			services.AddTransient<ITransliterationService, TransliterationService>();
			services.AddTransient<ICartService, CartService>();
			services.AddTransient<IOrderService, OrderService>();
			services.AddTransient<IDataSeedService, DataSeedService>();
			services.AddTransient<ISiteMapService, SiteMapService>();
			services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
			// services.AddTransient<IHostingEnvironment, MockHostingEnvironmentTesting>();

			services.AddSingleton<IConfiguration>(
				new ConfigurationBuilder()
				.AddJsonFile("appsettings.json", optional: false)
				.Build()
			);

			return services;
		}
	}
}
