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
using Microsoft.AspNetCore.Routing;

namespace Shevastream.Tests.ControllerTests
{
	/// <summary>
	/// Integration tests for HomeController
	/// </summary>
	public partial class BlogControllerTest
	{
		public BlogControllerTest()
		{
			// var services = Extensions.RegisterServices();

			// var mockHttpContext = new Mock<HttpContext>();
			// mockHttpContext
			// 	.Setup(http => http.User.Claims)
			// 	.Returns(new List<Claim> {
			// 		new Claim("UserId", int.MaxValue.ToString())
			// 	});

			// var mockHttpAccessor = new Mock<IHttpContextAccessor>();
			// mockHttpAccessor.Setup(accessor => accessor.HttpContext).Returns(mockHttpContext.Object);

			// services.Replace(
			// 	new ServiceDescriptor(
			// 		typeof(IHttpContextAccessor),
			// 		accessor => mockHttpAccessor.Object,
			// 		ServiceLifetime.Singleton
			// 	)
			// );

			// _serviceProvider = Extensions.RegisterServices().BuildServiceProvider();

			// In testing environment, controller does not have HttpContext.
			// As a result, all calls to Response trigger NullPointer exception
			// We need to manually set default values (or mock)
			// _controller.ControllerContext = new ControllerContext();
			// _controller.ControllerContext.HttpContext = new DefaultHttpContext();

			// _serviceProvider.GetRequiredService<IDataSeedService>().SeedData();

			// dataContext.Users.Add(
			// 	new User
			// 	{
			// 		Id = int.MaxValue,
			// 		PassHash = cryptoService.CalculateHash("test-password"),
			// 	}
			// );
			// dataContext.SaveChanges();
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
			var blogService = new Mock<IBlogService>();
			blogService
				.Setup(blog => blog.GetAllPostsAsync())
				.ReturnsAsync(
					hasContent ?
					new List<BlogPostViewModel> { new BlogPostViewModel() } :
					new List<BlogPostViewModel> { }
				);

			var controller = new BlogController(blogService.Object);

			// Act
			var result = await controller.Index();

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

		[Theory]
		[InlineData("Publish")]
		[InlineData("Edit")]
		public void AuthorizationEnabled(string method)
		{
			var type = typeof(BlogController);
			var methodInfo = type.GetMethod(method);
			var attributes = methodInfo.GetCustomAttributes(typeof(AuthorizeAttribute), true);
			Assert.True(attributes.Any());
		}

		[Theory]
		[InlineData(true)]
		[InlineData(false)]
		public async Task Publish(bool active)
		{
			// Arrange
			var blogPost = new BlogPost
			{
				Active = active,
				Title = "Test post",
				Content = "Something.. Something.. Something.. Something.. Something.. ",
				TitleUrl = "test-post"
			};
			var blogPostViewModel = BlogPostViewModel.FromBlogPost(blogPost);

			var blogService = new Mock<IBlogService>();
			blogService
				.Setup(blog => blog.UpdatePostAsync(blogPostViewModel))
				.ReturnsAsync(blogPost);
			blogService
				.Setup(blog => blog.CreatePostAsync(blogPostViewModel))
				.ReturnsAsync(blogPost);

			var dataContext = new Mock<IDataContext>();

			var controller = new BlogController(blogService.Object);

			// Act
			var result = await controller.Publish(blogPostViewModel);

			// Assert
			if (active)
			{
				var redirectResult = Assert.IsType<RedirectToRouteResult>(result);

				Assert.Equal(
					"test-post",
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

		[Theory]
		[InlineData(true, true)]
		[InlineData(true, false)]
		[InlineData(false, false)]
		public async Task Post(bool found, bool redirect)
		{
			// Arrange
			var blogService = new Mock<IBlogService>();
			blogService
				.Setup(blog => blog.GetPostByIdAsync(1, true))
				.ReturnsAsync(
					new BlogPostViewModel()
					{
						Id = 1,
						TitleUrl = "right-title-url"
					}
				);
			blogService
				.Setup(blog => blog.GetPostByIdAsync(-1, true))
				.ReturnsAsync(null);

			var controller = new BlogController(blogService.Object);

			// Act
			var result = await controller.Post(
				found ? 1 : -1,
				$"{(redirect ? "wrong" : "right")}-title-url"
			);

			// Assert
			if (found)
			{
				if (redirect)
				{
					var redirectResult = Assert.IsType<RedirectToRouteResult>(result);

					Assert.True(redirectResult.Permanent);
					Assert.Equal("Blog", redirectResult.RouteName);
					Assert.Equal(
						new RouteValueDictionary(new { id = 1, title = "right-title-url" }),
						redirectResult.RouteValues
					);
				}
				else
				{
					blogService.Verify(blog => blog.AddViewAsync(It.IsAny<BlogPostViewModel>()));

					var viewResult = Assert.IsType<ViewResult>(result);

					var model = Assert.IsAssignableFrom<BlogPostViewModel>(
						viewResult.ViewData.Model
					);

					Assert.Equal(1, model.Id);
				}
			}
			else
			{
				Assert.IsType<NotFoundResult>(result);
			}
		}

		[Theory]
		[InlineData(true, false)]
		[InlineData(false, true)]
		[InlineData(false, false)]
		public async Task Edit(bool @new, bool found)
		{
			// Arrange
			var blogService = new Mock<IBlogService>();
			blogService
				.Setup(blog => blog.GetPostByIdAsync(1, false))
				.ReturnsAsync(
					new BlogPostViewModel()
					{
						Id = 1
					}
				);
			blogService
				.Setup(blog => blog.GetPostByIdAsync(2, false))
				.ReturnsAsync(null);

			var controller = new BlogController(blogService.Object);

			// Act
			var result = await controller.Edit(
				@new ? -1 : (found ? 1 : 2),
				"title-url"
			);

			// Assert
			if (@new)
			{
				var viewResult = Assert.IsType<ViewResult>(result);

				var model = Assert.IsAssignableFrom<BlogPostViewModel>(
					viewResult.ViewData.Model
				);

				Assert.Equal(-1, model.Id);
			}
			else
			{
				if (found)
				{
					var viewResult = Assert.IsType<ViewResult>(result);

					var model = Assert.IsAssignableFrom<BlogPostViewModel>(
						viewResult.ViewData.Model
					);

					Assert.Equal(1, model.Id);
				}
				else
				{
					Assert.IsType<NotFoundResult>(result);
				}
			}
		}
	}
}
