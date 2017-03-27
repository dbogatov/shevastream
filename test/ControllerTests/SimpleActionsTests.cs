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
using Shevastream.Models;
using System.Reflection;
using Moq;

namespace Shevastream.Tests.ControllerTests
{
	[Collection("The Collection")]
	/// <summary>
	/// Integration tests for HomeController
	/// </summary>
	public partial class SimpleActionsTests
	{
		private readonly HomeController _home;
		private readonly StoreController _store;


		/// <summary>
		/// Provides registered service through dependency injection.
		/// </summary>
		private readonly IServiceProvider _serviceProvider;

		public SimpleActionsTests()
		{
			_serviceProvider = Extensions.RegisterServices().BuildServiceProvider();

			var siteMap = _serviceProvider.GetRequiredService<ISiteMapService>();
			var blogService = _serviceProvider.GetRequiredService<IBlogService>();
			var dataContext = _serviceProvider.GetRequiredService<DataContext>();
			var cartService = new Mock<ICartService>();
			
			cartService.Setup(cart => cart.GetTotalCost()).Returns(0);

			_home = new HomeController(siteMap, blogService);
			_store = new StoreController(dataContext, cartService.Object);

			// In testing environment, controller does not have HttpContext.
			// As a result, all calls to Response trigger NullPointer exception
			// We need to manually set default values (or mock)
			_home.ControllerContext = new ControllerContext();
			_home.ControllerContext.HttpContext = new DefaultHttpContext();
			_store.ControllerContext = new ControllerContext();
			_store.ControllerContext.HttpContext = new DefaultHttpContext();

			// Arrange			
			_serviceProvider.GetRequiredService<IDataSeedService>().SeedData();
		}

		[Theory]
		[InlineData(Controllers.Home, "FAQ")]
		[InlineData(Controllers.Home, "Contact")]
		[InlineData(Controllers.Home, "Profile")]
		[InlineData(Controllers.Store, "Order")]
		[InlineData(Controllers.Store, "ThankYou")]
		public async Task ActionTest(Controllers controllerEnum, string actionString)
		{
			// Arrange
			Controller controller;
			switch (controllerEnum)
			{
				case Controllers.Home:
					controller = _home;
					break;
				case Controllers.Store:
					controller = _store;
					break;
				default:
					throw new ArgumentException("Controller is undefined");
			}

			// Act
			Type controllerType = controller.GetType();
			MethodInfo action = controllerType.GetMethod(actionString, new Type[] { });
			var response = action.Invoke(controller, null);

			var result = (response is Task) ? (object)(await (Task<object>)response) : (object)response;

			// Assert
			var actionResult = Assert.IsAssignableFrom<IActionResult>(response);
			var viewResult = Assert.IsType<ViewResult>(actionResult);

			Assert.Equal(HttpStatusCode.OK.AsInt(), viewResult.StatusCode.Value);

			Assert.Null(viewResult.ViewData.Model);
		}

	}

	public enum Controllers
	{
		Home, Store
	}
}
