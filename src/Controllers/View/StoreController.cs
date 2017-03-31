using System.Linq;
using System.Threading.Tasks;
using Shevastream.Services;
using Shevastream.ViewModels.Store;
using Microsoft.AspNetCore.Mvc;
using Shevastream.Models;

namespace Shevastream.Controllers.View
{
	public class StoreController : Controller
	{
		private readonly IDataContext _context;
		private readonly ICartService _cart;
		private readonly IOrderService _order;

		public StoreController(IDataContext context, ICartService cart, IOrderService order)
		{
			_context = context;
			_cart = cart;
			_order = order;
		}

		public IActionResult Index()
		{
			return RedirectToActionPermanent("Index", "Home");
		}

		public IActionResult Order()
		{
			ViewBag.TotalCost = _cart.GetTotalCost();

			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Order(OrderViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return RedirectToAction("Order");
			}

			await _order.PutOrderAsync(model);

			return RedirectToAction("ThankYou");
		}

		public IActionResult Product(int? id)
		{
			if (id.HasValue)
			{
				if (_context.Products.Any(p => p.Id == id))
				{
					var product = _context.Products.First(p => p.Id == id);
					RouteData.Values["id"] = null;

					ViewData["isInCart"] = _cart.GetSimpleCart().Elements.Any(el => el.ProductId == id);

					return View("Detail", product);
				}

				return NotFound();
			}

			var products = _context.Products.AsEnumerable();
			return View(products);
		}

		public IActionResult Cart()
		{
			var model = _cart.GetCart();

			return View(model);
		}

		[HttpPost]
		public IActionResult Cart(CartElementViewModel element)
		{
			if (ModelState.IsValid)
			{
				_cart.UpdateCart(element);
			}

			return RedirectToAction("Cart");
		}

		public IActionResult ThankYou()
		{
			return View();
		}
	}
}
