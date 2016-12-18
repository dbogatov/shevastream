using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EShop.Services;
using EShop.ViewModels.Store;
using Microsoft.AspNetCore.Mvc;

namespace EShop.Controllers.View
{
	public class StoreController : Controller
	{
		private readonly DataContext _context;
		private readonly ICartService _cart;

		public StoreController(DataContext context, ICartService cart)
		{
			_context = context;
			_cart = cart;
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
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Order(
			[FromServices] IOrderService order,
			[FromForm] OrderViewModel model,
			CancellationToken requestAborted)
		{
			if (!ModelState.IsValid)
			{
				ViewBag.TotalCost = _cart.GetTotalCost();
				return View(model);
			}

			await order.PutOrderAsync(model);

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

		// CART

		public IActionResult Cart()
		{
			var model = _cart.GetCart();

			return View(model);
		}

		[HttpPost]
		[Route("Store/Cart/Update")]
		public IActionResult CartUpdate(CartElementViewModel element)
		{
			_cart.UpdateCart(element);

			return RedirectToAction("Cart");
		}

		[HttpPost]
		[Route("Store/Cart/Add")]
		public IActionResult CartAdd(CartElementViewModel element)
		{
			_cart.AddItem(element);

			return RedirectToAction("Cart");
		}

		[HttpPost]
		[Route("Store/Cart/Remove")]
		public IActionResult CartRemove(CartElementViewModel element)
		{
			_cart.RemoveItem(element);

			return RedirectToAction("Cart");
		}

		// END CART

		public IActionResult ThankYou()
		{
			return View();
		}
	}
}
