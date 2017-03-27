using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shevastream.Services;
using Xunit;
using Microsoft.AspNetCore.Http;
using Shevastream.Controllers.View;
using Shevastream.ViewModels.Blog;
using Shevastream.Tests;
using Shevastream.Extensions;

namespace StatusMonitor.Tests.ControllerTests
{
	[Collection("The Collection")]
	/// <summary>
	/// Integration tests for HomeController
	/// </summary>
	public partial class HomeControllerTest
	{
		private readonly HomeController _controller;

		/// <summary>
		/// Provides registered service through dependency injection.
		/// </summary>
		private readonly IServiceProvider _serviceProvider;

		public HomeControllerTest()
		{
			_serviceProvider = Extensions.RegisterServices().BuildServiceProvider();

			var siteMap = _serviceProvider.GetRequiredService<ISiteMapService>();
			var blogService = _serviceProvider.GetRequiredService<IBlogService>();

			_controller = new HomeController(siteMap, blogService);

			// In testing environment, controller does not have HttpContext.
			// As a result, all calls to Response trigger NullPointer exception
			// We need to manually set default values (or mock)
			_controller.ControllerContext = new ControllerContext();
			_controller.ControllerContext.HttpContext = new DefaultHttpContext();
		}

		[Fact]
		/// <summary>
		/// Checks that Index method returns proper model and status codes.
		/// </summary>
		public async Task IndexTest()
		{
			// Arrange			
			await _serviceProvider.GetRequiredService<IDataSeedService>().SeedDataAsync();
			
			// Act
			var result = await _controller.Index();

			// Assert
			var viewResult = Assert.IsType<ViewResult>(result);

			Assert.Equal(HttpStatusCode.OK.AsInt(), viewResult.StatusCode.Value);

			var model = Assert.IsAssignableFrom<IEnumerable<BlogPostViewModel>>(
				viewResult.ViewData.Model
			);

			Assert.NotEmpty(model);
		}

	}
}
