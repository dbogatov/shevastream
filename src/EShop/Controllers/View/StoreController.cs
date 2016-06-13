using System.Linq;
using EShop.Services;
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
            int model = _cart.GetTotalCost();
			
			return View((object)model);
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
