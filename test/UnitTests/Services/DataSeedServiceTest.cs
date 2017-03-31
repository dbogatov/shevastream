using Xunit;
using Shevastream.Models.Entities;
using Shevastream.Services;
using Shevastream.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using Moq;

namespace Shevastream.Tests.UnitTests.Services
{
	/// <summary>
	/// Unit tests for IDataSeedService
	/// </summary>
	public class DataSeedServiceTest
	{
		private readonly IDataContext _dataContext;

		public DataSeedServiceTest()
		{
			var serviceProvider = Extensions.RegisterServices().BuildServiceProvider();
			_dataContext = serviceProvider.GetRequiredService<IDataContext>();

		}

		[Fact]
		public void InitiallyNoValues()
		{
			// Assert
			Assert.Empty(_dataContext.Users);
			Assert.Empty(_dataContext.Products);
			Assert.Empty(_dataContext.BlogPosts);
			Assert.Empty(_dataContext.Orders);
			Assert.Empty(_dataContext.OrderProducts);
		}

		[Fact]
		/// <summary>
		/// Check if the service is able to seed some data into the empty data provider
		/// </summary>
		public async Task ServiceSeedsValuesToDataProvider()
		{
			// Arrange
			var dataSeedService = new DataSeedService(
				_dataContext,
				new Mock<ICryptoService>().Object
			);

			// Act
			await dataSeedService.SeedDataAsync();

			// Assert
			Assert.NotEmpty(_dataContext.Users);
			Assert.NotEmpty(_dataContext.Products);
			Assert.NotEmpty(_dataContext.BlogPosts);
			Assert.NotEmpty(_dataContext.Orders);
			Assert.NotEmpty(_dataContext.OrderProducts);
		}

		[Fact]
		/// <summary>
		/// Test that if correct data is already in place, service does not insert duplicates,
		/// or removes the whole data.
		/// </summary>
		public async Task ProperlyOverridesData()
		{
			// Arrange
			var dataSeedService = new DataSeedService(
				_dataContext,
				new Mock<ICryptoService>().Object
			);

			// Put some data as it were already there
			await _dataContext.Orders.AddRangeAsync(new List<Order> {
				new Order {
					Id = 3,
					Address = "100 Institute Road",
					CustomerName = "Dmytro",
					CustomerPhone = "+18577778350",
					CustomerEmail = "dmytro@dbogatov.org",
					ShipmentMethod = "К корпусу Шевченка",
					PaymentMethod = "Наличными",
					Comment = "Wanted to try blue one"
				}
			});
			await _dataContext.SaveChangesAsync();

			// Act
			await dataSeedService.SeedDataAsync();

			// Assert
			Assert.Equal(await _dataContext.Orders.CountAsync(), 1);
		}

		[Fact]
		public async Task NoDuplicates()
		{
			// Arrange
			var dataSeedService = new DataSeedService(
				_dataContext,
				new Mock<ICryptoService>().Object
			);

			// Act
			await dataSeedService.SeedDataAsync();
			await dataSeedService.SeedDataAsync();

			// Assert
			Assert.Equal(await _dataContext.Orders.CountAsync(), 1);
		}
	}
}
