using Xunit;
using Shevastream.Services;
using Shevastream.Models;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Collections.Generic;
using System.Linq;
using Shevastream.Models.Entities;
using Microsoft.AspNetCore.Http;
using Shevastream.ViewModels.Store;
using Newtonsoft.Json;

namespace Shevastream.Tests.UnitTests.Services
{
	public class CartServiceTest
	{
		private readonly CartViewModel _cookieValue;
		private readonly Mock<IRequestCookieCollection> _cookies;
		private readonly IDataContext _dataContext;
		private readonly Mock<IHttpContextAccessor> _httpAccessor;

		public CartServiceTest()
		{
			var serviceProvider = Extensions.RegisterServices().BuildServiceProvider();

			_dataContext = serviceProvider.GetRequiredService<IDataContext>();

			_dataContext.Products.AddRange(new List<Product> {
				new Product { Id = 1, Cost = 10 },
				new Product { Id = 2, Cost = 20 },
				new Product { Id = 3, Cost = 30 },
			});

			_dataContext.SaveChanges();

			_cookieValue = new CartViewModel
			{
				Elements = new List<CartElementViewModel> {
					new CartElementViewModel {
						ProductId = 1,
						Quantity = 2
					},
					new CartElementViewModel {
						ProductId = 2,
						Quantity = 3
					}
				}
			};

			_cookies = new Mock<IRequestCookieCollection>();
			_cookies
				.SetupGet(collection => collection[It.IsAny<string>()])
				.Returns(JsonConvert.SerializeObject(_cookieValue));

			_httpAccessor = new Mock<IHttpContextAccessor>();
			_httpAccessor
				.Setup(accessor => accessor.HttpContext.Request.Cookies)
				.Returns(_cookies.Object);
			_httpAccessor
				.Setup(accessor => accessor.HttpContext.Response.Cookies.Append(
					It.IsAny<string>(), It.IsAny<string>()
				));
			_httpAccessor
				.Setup(accessor => accessor.HttpContext.Response.Cookies.Delete(
					It.IsAny<string>()
				));
		}

		[Fact]
		public void GetsFullCart()
		{
			// Arrange
			var cartService = new CartService(
				_httpAccessor.Object,
				_dataContext
			);

			// Act
			var cart = cartService.GetCart();

			// Assert
			Assert.Equal(2, cart.Products.Count());
			Assert.True(cart.Products.Any(p => p.Product.Id == 1));
			Assert.True(cart.Products.Any(p => p.Product.Id == 2));
		}

		[Fact]
		public void GetsTotalCost()
		{
			// Arrange
			var cartService = new CartService(
				_httpAccessor.Object,
				_dataContext
			);

			// Act
			var totalCost = cartService.GetTotalCost();

			// Assert
			Assert.Equal(80, totalCost);
		}

		[Theory]
		[InlineData(true)]
		[InlineData(false)]
		public void UpdatesCart(bool changeElement)
		{
			// Arrange
			var cartService = new CartService(
				_httpAccessor.Object,
				new Mock<IDataContext>().Object
			);

			var newCart = _cookieValue;

			var element = new CartElementViewModel
			{
				ProductId = changeElement ? 2 : 3,
				Quantity = 3
			};

			if (changeElement)
			{
				newCart.Elements.Remove(
					newCart.Elements.First(el => el.ProductId == element.ProductId)
				);
			}

			newCart.Elements.Add(element);

			// Act
			cartService.UpdateCart(element);

			// Assert
			_httpAccessor.Verify(
				accessor => accessor.HttpContext.Response.Cookies.Append(
					It.IsAny<string>(),
					JsonConvert.SerializeObject(newCart)
				)
			);
		}

		[Fact]
		public void GetsSimpleCart()
		{
			// Arrange
			var cartService = new CartService(
				_httpAccessor.Object,
				new Mock<IDataContext>().Object
			);

			// Act
			var cart = cartService.GetSimpleCart();

			// Assert
			Assert.Equal(_cookieValue.Elements.Count(), cart.Elements.Count());
		}

		[Theory]
		[InlineData(true)]
		[InlineData(false)]
		public void VerifiesEmpty(bool shouldBeEmpty)
		{
			// Arrange
			if (shouldBeEmpty)
			{
				_cookies
					.SetupGet(collection => collection[It.IsAny<string>()])
					.Returns((string)null);
			}

			var cartService = new CartService(
				_httpAccessor.Object,
				new Mock<IDataContext>().Object
			);

			// Act
			var empty = cartService.IsCartEmpty();

			// Assert
			if (shouldBeEmpty)
			{
				Assert.True(empty);	
			}
			else
			{
				Assert.False(empty);	
			}
		}

		[Fact]
		public void EmptiesCart()
		{
			// Arrange
			var cartService = new CartService(
				_httpAccessor.Object,
				new Mock<IDataContext>().Object
			);

			// Act
			cartService.EmptyCart();

			// Assert
			_httpAccessor.Verify(
				accessor => accessor.HttpContext.Response.Cookies.Delete(
					It.IsAny<string>()				)
			);
		}
	}
}
