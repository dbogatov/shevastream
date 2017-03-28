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
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Shevastream.Tests.ControllerTests
{
	/// <summary>
	/// Integration tests for HomeController
	/// </summary>
	public partial class AccountControllerTest
	{
		private readonly AccountController _controller;

		public AccountControllerTest()
		{
			var cryptoService = new Mock<ICryptoService>();
			cryptoService
				.Setup(crypto => crypto.CalculateHash("good-password"))
				.Returns("good-hash");
			cryptoService
				.Setup(crypto => crypto.CalculateHash("bad-password"))
				.Returns("bad-hash");

			var users = new List<User> 
            { 
                new User { 
					PassHash = "good-hash" 
				}
            }.AsQueryable(); 
 
			var mockSet = new Mock<DbSet<User>>(); 
			mockSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(users.Provider); 
            mockSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(users.Expression); 
            mockSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(users.ElementType); 
            mockSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(() => users.GetEnumerator()); 

			var dataContext = new Mock<IDataContext>();
			dataContext
				.Setup(context => context.Users)
				.Returns(mockSet.Object);

			_controller = new AccountController(cryptoService.Object, dataContext.Object);

			_controller.ControllerContext.HttpContext = new DefaultHttpContext();
		}

		[Fact]
		/// <summary>
		/// Checks that Index method returns proper model and status codes.
		/// </summary>
		public void Login()
		{
			// Act
			var result = _controller.Login();

			// Assert
			var viewResult = Assert.IsType<ViewResult>(result);

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
		public void Logout()
		{
			// Arrange
			var mockAuth = new Mock<AuthenticationManager>();
			mockAuth
				.Setup(
					auth => auth
						.SignOutAsync("MyCookieMiddlewareInstance")
				);
			
			var mockContext = new Mock<HttpContext>();
			mockContext.Setup(context => context.Authentication).Returns(mockAuth.Object);

			_controller.ControllerContext.HttpContext = mockContext.Object;

			// Act
			var result = _controller.Logout();

			// Assert
			mockAuth.Verify(auth => auth.SignOutAsync("MyCookieMiddlewareInstance"));

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
		public void Authenticate(bool shouldSucceed)
		{
			// Arrange
			var mockForm = new Mock<IFormCollection>();
			mockForm.Setup(form => form["password"]).Returns(shouldSucceed ? "good-password" : "bad-password");

			var mockQuery = new Mock<IQueryCollection>();
			mockQuery.Setup(query => query["returnurl"]).Returns("/return");

			var mockAuth = new Mock<AuthenticationManager>();
			mockAuth
				.Setup(
					auth => auth
						.SignInAsync("MyCookieMiddlewareInstance", It.IsAny<ClaimsPrincipal>())
				);

			var mockContext = new Mock<HttpContext>();
			mockContext.Setup(context => context.Authentication).Returns(mockAuth.Object);
			mockContext.Setup(context => context.Request.Form).Returns(mockForm.Object);
			mockContext.Setup(context => context.Request.Query).Returns(mockQuery.Object);

			_controller.ControllerContext.HttpContext = mockContext.Object;

			// Act
			var result = _controller.Authenticate();

			// Assert
			mockAuth.Verify(
				auth => auth.SignInAsync("MyCookieMiddlewareInstance", It.IsAny<ClaimsPrincipal>()), 
				shouldSucceed ? Times.Once() : Times.Never()
			);

			if (shouldSucceed)
			{
				var redirectResult = Assert.IsType<RedirectResult>(result);

				Assert.Equal("/return", redirectResult.Url);
			}
			else
			{
				var viewResult = Assert.IsType<ViewResult>(result);

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
