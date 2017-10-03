using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Shevastream.Services;
using Xunit;
using Shevastream.Controllers.View;
using Shevastream.Models;
using System.Reflection;
using Moq;
using Microsoft.Extensions.Configuration;

namespace Shevastream.Tests.ControllerTests
{
	public partial class SimpleActionsTests
	{
		private readonly HomeController _home;
		private readonly StoreController _store;

		public SimpleActionsTests()
		{
			var cartService = new Mock<ICartService>();
			cartService.Setup(cart => cart.GetTotalCost()).Returns(0);

			_home = new HomeController(
				new Mock<ISiteMapService>().Object,
				new Mock<IBlogService>().Object,
				new Mock<IConfiguration>().Object
			);
			_store = new StoreController(
				new Mock<IDataContext>().Object,
				cartService.Object,
				new Mock<IOrderService>().Object
			);
		}

		[Theory]
		[InlineData(Controllers.Home, "FAQ")]
		[InlineData(Controllers.Home, "Contact")]
		[InlineData(Controllers.Home, "Profile")]
		[InlineData(Controllers.Store, "Order")]
		[InlineData(Controllers.Store, "ThankYou")]
		public async Task Action(Controllers controllerEnum, string actionString)
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

			Assert.Null(viewResult.ViewData.Model);
		}

	}

	public enum Controllers
	{
		Home, Store
	}
}
