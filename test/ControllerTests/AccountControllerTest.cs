using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shevastream.Services;
using Xunit;
using Microsoft.AspNetCore.Http;
using Shevastream.Controllers.View;
using Shevastream.Extensions;
using Moq;
using Shevastream.Models;
using Shevastream.ViewModels;
using System.Security.Claims;
using Microsoft.AspNetCore.Http.Authentication;
using Shevastream.Models.Entities;

namespace Shevastream.Tests.ControllerTests
{
	/// <summary>
	/// Integration tests for HomeController
	/// </summary>
	public partial class AccountControllerTest
	{
		private readonly AccountController _controller;

		/// <summary>
		/// Provides registered service through dependency injection.
		/// </summary>
		private readonly IServiceProvider _serviceProvider;

		public AccountControllerTest()
		{
			_serviceProvider = Extensions.RegisterServices().BuildServiceProvider();

			var cryptoService = _serviceProvider.GetRequiredService<ICryptoService>();
			var dataContext = _serviceProvider.GetRequiredService<DataContext>();

			_controller = new AccountController(cryptoService, dataContext);

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
		public void LoginTest()
		{
			// Act
			var result = _controller.Login();

			// Assert
			var viewResult = Assert.IsType<ViewResult>(result);

			Assert.Equal(HttpStatusCode.OK.AsInt(), viewResult.StatusCode.Value);

			var model = Assert.IsAssignableFrom<ReturnUrlViewModel>(
				viewResult.ViewData.Model
			);

			Assert.False(model.IsError);
			Assert.NotNull(model.ReturnUrl);
		}

		[Fact]
		/// <summary>
		/// Checks that Index method returns proper model and status codes.
		/// </summary>
		public void LogoutTest()
		{
			// Arrange
			var authenticated = true;

			var mockAuth = new Mock<AuthenticationManager>();
			mockAuth
				.Setup(
					auth => auth
						.SignOutAsync("MyCookieMiddlewareInstance")
				)
				.Returns(Task.CompletedTask)
				.Callback(() => authenticated = false);

			var mockContext = new Mock<HttpContext>();
			mockContext.Setup(context => context.Authentication).Returns(mockAuth.Object);

			_controller.ControllerContext.HttpContext = mockContext.Object;

			// Act
			var result = _controller.Logout();

			// Assert
			Assert.False(authenticated);

			var redirectResult = Assert.IsType<RedirectToActionResult>(result);

			Assert.Equal("Index", redirectResult.ActionName);
			Assert.Equal("Home", redirectResult.ControllerName);
		}

		[Theory]
		[InlineData(true)]
		[InlineData(false)]
		/// <summary>
		/// Checks that Index method returns proper model and status codes.
		/// </summary>
		public async Task AuthenticateTest(bool shouldAuthenticate)
		{
			// Arrange
			var authenticated = false;

			var dataContext = _serviceProvider.GetRequiredService<DataContext>();
			var cryptoService = _serviceProvider.GetRequiredService<ICryptoService>();
			dataContext.Users.Add(
				new User
				{
					Id = int.MaxValue,
					PassHash = cryptoService.CalculateHash("test-password"),
				}
			);
			await dataContext.SaveChangesAsync();

			var mockForm = new Mock<IFormCollection>();
			mockForm.Setup(form => form["password"]).Returns(shouldAuthenticate ? "test-password" : "wrong-password");

			var mockQuery = new Mock<IQueryCollection>();
			mockQuery.Setup(query => query["returnurl"]).Returns("/return");

			var mockAuth = new Mock<AuthenticationManager>();
			mockAuth
				.Setup(
					auth => auth
						.SignInAsync("MyCookieMiddlewareInstance", It.IsAny<ClaimsPrincipal>())
				)
				.Returns(Task.CompletedTask)
				.Callback(() => authenticated = true);

			var mockContext = new Mock<HttpContext>();
			mockContext.Setup(context => context.Authentication).Returns(mockAuth.Object);
			mockContext.Setup(context => context.Request.Form).Returns(mockForm.Object);
			mockContext.Setup(context => context.Request.Query).Returns(mockQuery.Object);

			_controller.ControllerContext.HttpContext = mockContext.Object;

			// Act
			var result = _controller.Authenticate();

			// Assert
			Assert.True(shouldAuthenticate ? authenticated : !authenticated);

			if (shouldAuthenticate)
			{
				var redirectResult = Assert.IsType<RedirectResult>(result);

				Assert.Equal("/return", redirectResult.Url);
			}
			else
			{
				var viewResult = Assert.IsType<ViewResult>(result);

				Assert.Equal(HttpStatusCode.Unauthorized.AsInt(), viewResult.StatusCode.Value);

				var model = Assert.IsAssignableFrom<ReturnUrlViewModel>(
					viewResult.ViewData.Model
				);

				Assert.True(model.IsError);
				Assert.Equal("Wrong password", model.Error);
				Assert.Equal("/return", model.ReturnUrl);
			}
		}
	}
}
