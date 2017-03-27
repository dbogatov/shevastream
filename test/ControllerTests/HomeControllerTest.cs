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
using Shevastream.Extensions;
using Moq;
using System.Linq;

namespace Shevastream.Tests.ControllerTests
{
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

			var blogService = _serviceProvider.GetRequiredService<IBlogService>();
			var siteMap = new Mock<ISiteMapService>();
			siteMap.
				Setup(map => map.GetSiteMap()).
				Returns(new SiteMap { 
					Items = new List<SiteMapItem> {
						new SiteMapItem {
							Loc = new Uri("https://shevastream.com"),
							LastMod = DateTime.Now,
							ChangeFreq = ChangeFrequency.Monthly
						}
					} 
				});
			

			_controller = new HomeController(siteMap.Object, blogService);

			// In testing environment, controller does not have HttpContext.
			// As a result, all calls to Response trigger NullPointer exception
			// We need to manually set default values (or mock)
			_controller.ControllerContext = new ControllerContext();
			_controller.ControllerContext.HttpContext = new DefaultHttpContext();

			// Arrange
			_serviceProvider.GetRequiredService<IDataSeedService>().SeedData();
		}

		[Fact]
		/// <summary>
		/// Checks that Index method returns proper model and status codes.
		/// </summary>
		public async Task IndexTest()
		{
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

		[Fact]
		public void SiteMapTest()
		{	
			// Act
			var result = _controller.SiteMap();

			// Assert
			var siteMapResult = Assert.IsType<SiteMapResult>(result);

			var model = Assert.IsType<SiteMap>(
				siteMapResult.SiteMap
			);

			Assert.NotEmpty(model.Items);
		}

		[Fact]
		public void PrivacyTest()
		{	
			// Act
			var result = _controller.Privacy();

			// Assert
			var redirectResult = Assert.IsType<RedirectToRouteResult>(result);

			Assert.True(redirectResult.Permanent);

			Assert.True(redirectResult.RouteValues.Keys.Any(key => key == "id"));

			var id = Assert.IsType<int>(redirectResult.RouteValues["id"]);

			Assert.NotEqual(0, id);
		}

	}
}
