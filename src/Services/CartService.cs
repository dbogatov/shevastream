using System.Collections.Generic;
using System.Linq;
using Shevastream.ViewModels.Store;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Shevastream.Models;

namespace Shevastream.Services
{
	public interface ICartService
	{
		FullCartViewModel GetCart();
		int GetTotalCost();
		void UpdateCart(CartElementViewModel element);
		CartViewModel GetSimpleCart();
		bool IsCartEmpty();
		void EmptyCart();
	}

	public class CartService : ICartService
	{
		private readonly string CART_COOKIE_NAME = "Cart";

		private readonly HttpContext _http;
		private readonly IDataContext _context;

		private CartViewModel _cart;
		private CartViewModel Cart
		{
			get
			{
				if (_cart != null)
				{
					return _cart;
				}
				else if (_http.Request.Cookies[CART_COOKIE_NAME] != null)
				{
					_cart = JsonConvert.
						DeserializeObject<CartViewModel>(
							_http.Request.Cookies[CART_COOKIE_NAME]
						);
					return _cart;
				}
				else
				{
					_cart = new CartViewModel
					{
						Elements = new List<CartElementViewModel>()
					};

					_http.
						Response.
						Cookies.
						Append(
							CART_COOKIE_NAME,
							JsonConvert.SerializeObject(_cart)
						);

					return _cart;
				}
			}
			set
			{
				_cart = value;
				_http.
					Response.
					Cookies.
					Append(
						CART_COOKIE_NAME,
						JsonConvert.SerializeObject(_cart)
					);
			}
		}

		public CartService(IHttpContextAccessor http, IDataContext context)
		{
			_http = http.HttpContext;
			_context = context;
		}

		/// <summary>
		/// Returns a cart model for the user
		/// </summary>
		/// <returns>Model with empty product list if there is no cart; list of products in the cart otherwise</returns>
		public FullCartViewModel GetCart()
		{
			FullCartViewModel model = new FullCartViewModel();

			if (_http.Request.Cookies[CART_COOKIE_NAME] == null)
			{
				model.Products = new List<FullCartElementViewModel>();
			}
			else
			{
				var elements = JsonConvert.
					DeserializeObject<CartViewModel>(
						_http.Request.Cookies[CART_COOKIE_NAME]
					).Elements;

				var products = _context.Products.AsEnumerable();

				model.Products = (
					from element in elements
					join prod in products on element.ProductId equals prod.Id
					select new FullCartElementViewModel
					{
						Product = prod,
						Quantity = element.Quantity
					}).ToList();
			}

			return model;
		}

		public int GetTotalCost()
		{
			int model;

			if (_http.Request.Cookies[CART_COOKIE_NAME] == null)
			{
				model = 0;
			}
			else
			{
				var elements = JsonConvert.
					DeserializeObject<CartViewModel>(
						_http.Request.Cookies[CART_COOKIE_NAME]
					).Elements;

				var products = _context.Products.AsEnumerable();

				model = (
					from element in elements
					join prod in products on element.ProductId equals prod.Id
					select prod.Cost * element.Quantity
				).Sum();
			}

			return model;
		}

		public void UpdateCart(CartElementViewModel element)
		{
			if (Cart.Elements.Any(el => el.ProductId == element.ProductId))
			{
				var toRemove = Cart.Elements.First(el => el.ProductId == element.ProductId);
				Cart.Elements.Remove(toRemove);
				if (element.Quantity > 0)
				{
					Cart.Elements.Add(element);
				}
			}
			else
			{
				element.Quantity = 1;
				Cart.Elements.Add(element);
			}
			SaveChanges();
		}

		public CartViewModel GetSimpleCart()
		{
			return Cart;
		}

		public void RemoveItem(CartElementViewModel element)
		{
			if (Cart.Elements.Any(el => el.ProductId == element.ProductId))
			{
				var toRemove = Cart.Elements.First(el => el.ProductId == element.ProductId);
				Cart.Elements.Remove(toRemove);
			}
			SaveChanges();
		}

		public bool IsCartEmpty()
		{
			return _http.Request.Cookies[CART_COOKIE_NAME] == null;
		}

		public void EmptyCart()
		{
			_http.Response.Cookies.Delete(CART_COOKIE_NAME);
		}

		private void SaveChanges()
		{
			Cart = Cart;
		}
	}
}
