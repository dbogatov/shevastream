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
using Shevastream.Models;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Security.Claims;
using Shevastream.Models.Entities;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;

namespace Shevastream.Tests.ControllerTests
{
	/// <summary>
	/// Integration tests for HomeController
	/// </summary>
	public partial class BlogControllerTest
	{
		private readonly BlogController _controller;

		/// <summary>
		/// Provides registered service through dependency injection.
		/// </summary>
		private readonly IServiceProvider _serviceProvider;

		public BlogControllerTest()
		{
			var services = Extensions.RegisterServices();

			var mockHttpContext = new Mock<HttpContext>();
			mockHttpContext
				.Setup(http => http.User.Claims)
				.Returns(new List<Claim> {
					new Claim("UserId", int.MaxValue.ToString())
				});

			var mockHttpAccessor = new Mock<IHttpContextAccessor>();
			mockHttpAccessor.Setup(accessor => accessor.HttpContext).Returns(mockHttpContext.Object);

			services.Replace(
				new ServiceDescriptor(
					typeof(IHttpContextAccessor),
					accessor => mockHttpAccessor.Object,
					ServiceLifetime.Singleton
				)
			);

			_serviceProvider = services.BuildServiceProvider();

			var blogService = _serviceProvider.GetRequiredService<IBlogService>();
			var cryptoService = _serviceProvider.GetRequiredService<ICryptoService>();
			var dataContext = _serviceProvider.GetRequiredService<DataContext>();

			_controller = new BlogController(dataContext, blogService);

			// In testing environment, controller does not have HttpContext.
			// As a result, all calls to Response trigger NullPointer exception
			// We need to manually set default values (or mock)
			_controller.ControllerContext = new ControllerContext();
			_controller.ControllerContext.HttpContext = new DefaultHttpContext();

			_serviceProvider.GetRequiredService<IDataSeedService>().SeedData();

			dataContext.Users.Add(
				new User
				{
					Id = int.MaxValue,
					PassHash = cryptoService.CalculateHash("test-password"),
				}
			);
			dataContext.SaveChanges();
		}

		[Theory]
		[InlineData(true)]
		[InlineData(true)]
		/// <summary>
		/// Checks that Index method returns proper model and status codes.
		/// </summary>
		public async Task Index(bool hasContent)
		{
			// Arrange
			if (hasContent)
			{
				_serviceProvider.GetRequiredService<IDataSeedService>().SeedData();
			}

			// Act
			var result = await _controller.Index();

			// Assert
			var viewResult = Assert.IsType<ViewResult>(result);

			Assert.Equal(
				(hasContent ? HttpStatusCode.OK : HttpStatusCode.NoContent).AsInt(),
				viewResult.StatusCode.Value
			);

			var model = Assert.IsAssignableFrom<IEnumerable<BlogPostViewModel>>(
				viewResult.ViewData.Model
			);

			if (hasContent)
			{
				Assert.NotEmpty(model);
			}
			else
			{
				Assert.Empty(model);
			}
		}

		[Fact]
		public void PublishAuthorize()
		{
			var type = _controller.GetType();
			var methodInfo = type.GetMethod("Publish", new Type[] { typeof(BlogPostViewModel) });
			var attributes = methodInfo.GetCustomAttributes(typeof(AuthorizeAttribute), true);
			Assert.True(attributes.Any());
		}

		[Theory]
		[InlineData(true)]
		[InlineData(false)]
		public async Task Publish(bool active)
		{
			// Arrange
			var blogPost = new BlogPostViewModel
			{
				Active = active,
				Title = "Test post",
				Content = "Something.. Something.. Something.. Something.. Something.. "
			};

			// Act
			var result = await _controller.Publish(blogPost);

			// Assert
			if (active)
			{
				var redirectResult = Assert.IsType<RedirectToRouteResult>(result);

				Assert.Equal(
					_serviceProvider
						.GetRequiredService<IBlogService>()
						.GenerateUrlFromTitle(blogPost.Title),
					redirectResult.RouteValues["title"]
				);

				Assert.InRange(
					Convert.ToInt32(redirectResult.RouteValues["id"]),
					0, int.MaxValue
				);
			}
			else
			{
				var redirectResult = Assert.IsType<RedirectToActionResult>(result);

				Assert.Equal("Index", redirectResult.ActionName);
			}
		}
	}
}
