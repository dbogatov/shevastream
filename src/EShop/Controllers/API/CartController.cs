using EShop.ViewModels.Store;
using Microsoft.AspNetCore.Mvc;
using EShop.Services;

namespace EShop.Controllers.API
{

    [Produces("application/json")]
	[Route("api/Cart")]
	public class CartController : Controller
	{
		private readonly ICartService _cart;

		public CartController(ICartService cart)
		{
			_cart = cart;
		}

		// POST api/Cart
		[HttpPost]
		public void Post(CartElementViewModel element)
		{
			_cart.UpdateCart(element);
		}

		// GET api/Cart
		[HttpGet]
		public CartViewModel Get()
		{
			return _cart.GetSimpleCart();
		}

		// PUT api/Cart
		[HttpPut]
		public void Put(CartElementViewModel element)
		{
            _cart.AddItem(element);
        }
		
		// DELETE api/Cart
		[HttpDelete]
		public void Delete(CartElementViewModel element)
		{
			_cart.RemoveItem(element);
        }
	}
}