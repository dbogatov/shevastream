using System;
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
			return View();
		}

		public IActionResult FAQ()
		{
			return View();
		}

		public IActionResult Contact()
		{

			return View();
		}

		public IActionResult Profile()
		{

			return View();
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

		public IActionResult ThankYou()
		{

			return View();
		}

		public IActionResult Error()
		{
			return View();
		}
	}
}
