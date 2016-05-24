using Newtonsoft.Json;
using EShop.ViewModels.Store;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace EShop.Controllers.API
{

	[Produces("application/json")]
	[Route("api/Cart")]
	public class CartController : Controller
	{
		private readonly string CART_COOKIE_NAME = "Cart";

		private readonly HttpContext _http;
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

		public CartController(IHttpContextAccessor http)
		{
			_http = http.HttpContext;
		}

		// POST api/Cart
		[HttpPost]
		public void Post(CartElementViewModel element)
		{
			if (Cart.Elements.Any(el => el.ProductId == element.ProductId))
			{
                var toRemove = Cart.Elements.First(el => el.ProductId == element.ProductId);
                Cart.Elements.Remove(toRemove);
				Cart.Elements.Add(element);
			}
			SaveChanges();
		}

		// GET api/Cart
		[HttpGet]
		public CartViewModel Get()
		{
			return Cart;
		}

		// PUT api/Cart
		[HttpPut]
		public void Put(CartElementViewModel element)
		{
			Cart.Elements.Add(element);
            SaveChanges();
        }
		
		// DELETE api/Cart
		[HttpDelete]
		public void Delete(CartElementViewModel element)
		{
			if (Cart.Elements.Any(el => el.ProductId == element.ProductId))
			{
                var toRemove = Cart.Elements.First(el => el.ProductId == element.ProductId);
                Cart.Elements.Remove(toRemove);
			}
			SaveChanges();
        }
		
		[NonAction]
		private void SaveChanges()
		{
            Cart = Cart;
            //var cart = Cart;
            //Cart = cart;
        }
	}
}