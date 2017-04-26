using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Shevastream.Services;
using Xunit;
using Shevastream.Controllers.View;
using Shevastream.ViewModels.Blog;
using Moq;
using System.Linq;
using Shevastream.Models;
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
		[Theory]
		[InlineData(true)]
		[InlineData(true)]
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
