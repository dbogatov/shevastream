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
		/// <summary>
		/// Returns a full cart model (includes list of products)
		/// </summary>
		/// <returns>Full cart model</returns>
		FullCartViewModel GetCart();

		/// <summary>
		/// Returns a total cost of all items in the cart
		/// </summary>
		/// <returns>Cart total cost</returns>
		int GetTotalCost();

		/// <summary>
		/// Update cart with a new element
		/// </summary>
		/// <param name="element">Element to be added.removed/updated</param>
		void UpdateCart(CartElementViewModel element);

		/// <summary>
		/// Returns a simplified cart model (includes list of product IDs)
		/// </summary>
		/// <returns>Simplified cart model</returns>
		CartViewModel GetSimpleCart();

		/// <summary>
		/// Checks if the cart is empty or does not exist
		/// </summary>
		/// <returns>False if cart contains any items, true otherwise</returns>
		bool IsCartEmpty();

		/// <summary>
		/// Empties the cart
		/// </summary>
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
