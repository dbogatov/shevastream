using Xunit;
using Shevastream.Models.Entities;
using Shevastream.Services;
using Shevastream.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace Shevastream.Tests.UnitTests.Services
{
	/// <summary>
	/// Unit tests for IDataSeedService
	/// </summary>
	public class DataSeedServiceTest
	{
		private readonly IDataContext _dataContext;
		private readonly IDataSeedService _dataSeed;

		public DataSeedServiceTest()
		{
			var serviceProvider = Extensions.RegisterServices().BuildServiceProvider();
			_dataContext = serviceProvider.GetRequiredService<IDataContext>();

			var mockBlogService = new Mock<IBlogService>();
			mockBlogService
				.Setup(mock => mock.GenerateUrlFromTitle(It.IsAny<string>()))
				.Returns("url-title");
			mockBlogService
				.Setup(mock => mock.GeneratePreview(It.IsAny<string>()))
				.Returns("<p>preview</p>");

			_dataSeed = new DataSeedService(
				serviceProvider.GetRequiredService<IConfiguration>(),
				_dataContext,
				new Mock<ICryptoService>().Object,
				mockBlogService.Object,
				new Mock<ILogger<DataSeedService>>().Object
			);
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
			// Act
			await _dataSeed.SeedDataAsync();

			// Assert
			Assert.NotEmpty(_dataContext.Users);
			Assert.NotEmpty(_dataContext.Products);
			Assert.NotEmpty(_dataContext.BlogPosts);
		}

		[Fact]
		/// <summary>
		/// Test that if correct data is already in place, service does not insert duplicates,
		/// or removes the whole data.
		/// </summary>
		public async Task ProperlyOverridesData()
		{
			// Arrange
			await _dataContext.Users.AddAsync(new User { Id = int.MaxValue });
			await _dataContext.SaveChangesAsync();

			// Act
			await _dataSeed.SeedDataAsync();

			// Assert
			Assert.Equal(await _dataContext.Users.CountAsync(), 3);
		}

		[Fact]
		public async Task NoDuplicates()
		{
			// Act
			await _dataSeed.SeedDataAsync();
			await _dataSeed.SeedDataAsync();

			// Assert
			Assert.Equal(await _dataContext.Users.CountAsync(), 3);
		}
	}
}
