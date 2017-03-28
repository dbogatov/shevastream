using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Shevastream.Services;
using Xunit;
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

		public HomeControllerTest()
		{
			var blogService = new Mock<IBlogService>();
			blogService
				.Setup(blog => blog.GetLatestPostsAsync(It.IsAny<int>()))
				.ReturnsAsync(new List<BlogPostViewModel> { new BlogPostViewModel() });

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
			
			_controller = new HomeController(siteMap.Object, blogService.Object);
		}

		[Fact]
		/// <summary>
		/// Checks that Index method returns proper model and status codes.
		/// </summary>
		public async Task Index()
		{
			// Act
			var result = await _controller.Index();

			// Assert
			var viewResult = Assert.IsType<ViewResult>(result);

			var model = Assert.IsAssignableFrom<IEnumerable<BlogPostViewModel>>(
				viewResult.ViewData.Model
			);

			Assert.NotEmpty(model);
		}

		[Fact]
		public void SiteMap()
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
		public void Privacy()
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
