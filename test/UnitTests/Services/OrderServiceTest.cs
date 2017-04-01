using Xunit;
using Shevastream.Services;
using Shevastream.Models;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Collections.Generic;
using System.Linq;
using Shevastream.Models.Entities;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Shevastream.ViewModels.Store;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;

namespace Shevastream.Tests.UnitTests.Services
{
	public class OrderServiceTest
	{
		private readonly IDataContext _dataContext;
		private readonly Mock<IHttpContextAccessor> _httpAccessor;
		private readonly Mock<ICartService> _cartService;
		private readonly Mock<IPushService> _pushService;

		public OrderServiceTest()
		{
			var serviceProvider = Extensions.RegisterServices().BuildServiceProvider();

			var prodcut = new Product
			{
				Id = 1
			};

			_dataContext = serviceProvider.GetRequiredService<IDataContext>();
			_dataContext.Products.Add(prodcut);
			_dataContext.SaveChanges();

			_httpAccessor = new Mock<IHttpContextAccessor>();
			_httpAccessor
				.Setup(accessor => accessor.HttpContext.Response.Cookies.Append(
					It.IsAny<string>(), It.IsAny<string>()
				));

			_cartService = new Mock<ICartService>();
			_cartService.Setup(cart => cart.EmptyCart());
			_cartService.Setup(cart => cart.IsCartEmpty()).Returns(false);
			_cartService
				.Setup(cart => cart.GetCart())
				.Returns(new FullCartViewModel
				{
					Products = new List<FullCartElementViewModel> {
						new FullCartElementViewModel {
							Product = prodcut,
							Quantity = 1
						}
					}
				});

			_pushService = new Mock<IPushService>();
			_pushService
				.Setup(push => push.SendOrderAsync(
					It.IsAny<string>(),
					It.IsAny<string>(),
					It.IsAny<string>(),
					It.IsAny<IEnumerable<Product>>()
				))
				.Returns(Task.CompletedTask);
		}

		[Fact]
		public async Task HandlesOrder()
		{
			// Arrange
			var orderService = new OrderService(
				_dataContext,
				_pushService.Object,
				_httpAccessor.Object,
				_cartService.Object,
				new Mock<ILogger<OrderService>>().Object
			);

			var order = new OrderViewModel
			{
				CustomerName = "John Doe",
				CustomerPhone = "875 369 5656",
				CustomerEmail = "jdoe@example.com",
				ShipmentMethod = "On campus",
				PaymentMethod = "Credit card"
			};

			await _pushService.Object.SendOrderAsync("", "", "", new List<Product>());

			// Act
			var orderId = await orderService.PutOrderAsync(order);

			// Assert
			
			Assert.True(_dataContext.Orders.Any(o => o.Id == orderId));
			Assert.Equal("John Doe", _dataContext.Orders.First(o => o.Id == orderId).CustomerName);

			Assert.True(_dataContext.OrderProducts.Any(op => op.OrderId == orderId && op.ProductId == 1));

			_pushService.Verify(push => push.SendOrderAsync(
				It.IsAny<string>(),
				"John Doe",
				"jdoe@example.com",
				It.Is<IEnumerable<Product>>(prods => prods.Count() == 1 && prods.Any(p => p.Id == 1))
			));

			_cartService.Verify(cart => cart.EmptyCart());

			_httpAccessor.Verify(
				accessor => accessor.HttpContext.Response.Cookies.Append(
					It.IsAny<string>(),
					JsonConvert.SerializeObject(new OrderUserData
					{
						Name = order.CustomerName,
						Email = order.CustomerEmail,
						Address = order.Address,
						Phone = order.CustomerPhone
					})
				)
			);



		}
	}
}

