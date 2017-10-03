using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Shevastream.Services;
using Xunit;
using Shevastream.Controllers.View;
using Moq;
using System.Linq;
using Shevastream.Models;
using Shevastream.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Shevastream.ViewModels.Store;
using Microsoft.AspNetCore.Routing;

namespace Shevastream.Tests.ControllerTests
{
	public partial class StoreControllerTest
	{
		private readonly StoreController _controller;
		private readonly Mock<IOrderService> _orderService;
		private readonly Mock<ICartService> _cartService;

		public StoreControllerTest()
		{
			var products = new List<Product>
			{
				new Product {
					Id = 1
				},
				new Product {
					Id = 2
				}
			}.AsQueryable();

			var mockSet = new Mock<DbSet<Product>>();
			mockSet.As<IQueryable<Product>>().Setup(m => m.Provider).Returns(products.Provider);
			mockSet.As<IQueryable<Product>>().Setup(m => m.Expression).Returns(products.Expression);
			mockSet.As<IQueryable<Product>>().Setup(m => m.ElementType).Returns(products.ElementType);
			mockSet.As<IQueryable<Product>>().Setup(m => m.GetEnumerator()).Returns(() => products.GetEnumerator());

			var dataContext = new Mock<IDataContext>();
			dataContext
				.Setup(context => context.Products)
				.Returns(mockSet.Object);

			_cartService = new Mock<ICartService>();
			_cartService
				.Setup(cart => cart.GetTotalCost())
				.Returns(10);
			_cartService
				.Setup(cart => cart.GetSimpleCart())
				.Returns(new CartViewModel
				{
					Elements = new List<CartElementViewModel>() {
						new CartElementViewModel {
							ProductId = 1
						}
					}
				});
			_cartService
				.Setup(cart => cart.GetCart())
				.Returns(new FullCartViewModel());

			_orderService = new Mock<IOrderService>();

			_controller = new StoreController(dataContext.Object, _cartService.Object, _orderService.Object);

			_controller.ControllerContext = new ControllerContext { RouteData = new RouteData() };
		}

		[Fact]
		public void Index()
		{
			// Act
			var result = _controller.Index();

			// Assert
			var redirectResult = Assert.IsType<RedirectToActionResult>(result);

			Assert.True(redirectResult.Permanent);
			Assert.Equal("Home", redirectResult.ControllerName);
			Assert.Equal("Index", redirectResult.ActionName);
		}

		[Fact]
		public void OrderPage()
		{
			// Act
			var result = _controller.Order();

			// Assert
			var viewResult = Assert.IsType<ViewResult>(result);

			Assert.Equal(10, Convert.ToInt32(viewResult.ViewData["TotalCost"]));
		}

		[Theory]
		[InlineData(true)]
		[InlineData(false)]
		public async Task OrderAction(bool shouldSucceed)
		{
			// Arrange
			if (!shouldSucceed)
			{
				_controller.ModelState.AddModelError("Key", "Some error");
			}

			// Act
			var result = await _controller.Order(new OrderViewModel());

			// Assert
			var redirectResult = Assert.IsType<RedirectToActionResult>(result);

			Assert.False(redirectResult.Permanent);
			Assert.Equal(shouldSucceed ? "ThankYou" : "Order", redirectResult.ActionName);

			_orderService.Verify(
				orderService => orderService.PutOrderAsync(It.IsAny<OrderViewModel>()),
				shouldSucceed ? Times.Once() : Times.Never()
			);
		}

		[Theory]
		[InlineData(true, true, true)]
		[InlineData(true, true, false)]
		[InlineData(true, false, false)]
		[InlineData(false, false, false)]
		public void Product(bool idProvided, bool found, bool inCart)
		{
			// Arrange
			var id = idProvided ? (int?)(found ? (inCart ? 1 : 2) : -1) : null;

			// Act
			var result = _controller.Product(id);

			// Assert
			if (idProvided)
			{
				if (found)
				{
					var viewResult = Assert.IsType<ViewResult>(result);

					var model = Assert.IsAssignableFrom<Product>(
						viewResult.ViewData.Model
					);

					Assert.InRange(model.Id, 1, 2);

					Assert.Equal("Detail", viewResult.ViewName);

					if (inCart)
					{
						Assert.True(Convert.ToBoolean(viewResult.ViewData["isInCart"]));
					}
				}
				else
				{
					Assert.IsType<NotFoundResult>(result);
				}
			}
			else
			{
				var viewResult = Assert.IsType<ViewResult>(result);

				Assert.IsAssignableFrom<IEnumerable<Product>>(
					viewResult.ViewData.Model
				);
			}
		}

		[Fact]
		public void CartPage()
		{
			// Act
			var result = _controller.Cart();

			// Assert
			var viewResult = Assert.IsType<ViewResult>(result);

			Assert.IsAssignableFrom<FullCartViewModel>(
				viewResult.ViewData.Model
			);
		}

		[Fact]
		public void CartAction()
		{
			// Act
			var result = _controller.Cart(new CartElementViewModel());

			// Assert
			var redirectResult = Assert.IsType<RedirectToActionResult>(result);

			Assert.Equal("Cart", redirectResult.ActionName);

			_cartService.Verify(cart => cart.UpdateCart(It.IsAny<CartElementViewModel>()));
		}
	}
}
