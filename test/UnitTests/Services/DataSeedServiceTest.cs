using System;
using Xunit;
using Shevastream.Models.Entities;
using Shevastream.Services;
using Shevastream.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Shevastream.Tests.UnitTests.Services
{
	/// <summary>
	/// Unit tests for IDataSeedService
	/// </summary>
	public class DataSeedServiceTest
	{
		/// <summary>
		/// Provides registered service through dependency injection.
		/// </summary>
		private readonly IServiceProvider _serviceProvider;

		/// <summary>
		/// Sets up service provider for the test.
		/// For this particular test set we need DataContext (using inMemory data provider), the IDataSeedService
		/// which we will test and IMetricService needed for IDataSeedService.
		/// </summary>
		public DataSeedServiceTest()
		{
			_serviceProvider = Extensions.RegisterServices().BuildServiceProvider();
		}

		[Fact]
		/// <summary>
		/// Check if the service is able to seed some data into the empty data provider
		/// </summary>
		public async Task ServiceSeedsValuesToDataProvider()
		{
			// Pull required services
			var dataContext = _serviceProvider.GetRequiredService<DataContext>();
			var dataSeedService = _serviceProvider.GetRequiredService<IDataSeedService>();

			// Check if the data provider is initially empty
			Assert.Empty(dataContext.Users);
			Assert.Empty(dataContext.Products);
			Assert.Empty(dataContext.BlogPosts);
			Assert.Empty(dataContext.Orders);
			Assert.Empty(dataContext.OrderProducts);

			// Seed initial data
			await dataSeedService.SeedDataAsync();

			// Check if the data is seeded
			Assert.NotEmpty(dataContext.Users);
			Assert.NotEmpty(dataContext.Products);
			Assert.NotEmpty(dataContext.BlogPosts);
			Assert.NotEmpty(dataContext.Orders);
			Assert.NotEmpty(dataContext.OrderProducts);
		}

		[Fact]
		/// <summary>
		/// Test that if correct data is already in place, service does not insert duplicates,
		/// or removes the whole data.
		/// </summary>
		public async Task ProperlyUpdatesData()
		{
			// Pull required services
			var dataContext = _serviceProvider.GetRequiredService<DataContext>();
			var dataSeedService = _serviceProvider.GetRequiredService<IDataSeedService>();

			// Put some data as it were already there
			await dataContext.Orders.AddRangeAsync(new List<Order> {
				new Order {
					Id = 1,
					Address = "100 Institute Road",
					CustomerName = "Dmytro",
					CustomerPhone = "+18577778350",
					CustomerEmail = "dmytro@dbogatov.org",
					ShipmentMethod = "К корпусу Шевченка",
					PaymentMethod = "Наличными",
					Comment = "Wanted to try blue one"
				}
			});

			await dataContext.SaveChangesAsync();

			// Seed initial data
			await dataSeedService.SeedDataAsync();

			// Check that we have the same number of records for the entity
			Assert.Equal(await dataContext.Orders.CountAsync(), 1);


			// Remove one entity, now there are not as many as required items
			dataContext.Orders.Remove(
				await dataContext.Orders.FindAsync(1)
			);

			await dataContext.SaveChangesAsync();

			// Seed initial data
			await dataSeedService.SeedDataAsync();

			// Check that we have the same number of records for the entity
			Assert.Equal(await dataContext.Orders.CountAsync(), 1);
		}
	}
}
