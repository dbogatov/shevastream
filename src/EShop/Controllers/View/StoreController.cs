using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace EShop.Controllers.View
{
	public class StoreController : Controller
	{
		private readonly DataContext _context;

		public StoreController(DataContext context)
		{
			_context = context;
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

		public IActionResult Order(int? id)
		{
            return View(
                _context.Products.FirstOrDefault(p => p.Id == id)
			);
		}

		public IActionResult Product(int? id)
		{
			if (id.HasValue)
			{
				if (_context.Products.Any(p => p.Id == id))
				{
					var product = _context.Products.First(p => p.Id == id);
					return View("Detail", product);
				}

				return NotFound();
			}

			var products = _context.Products.AsEnumerable();
			return View(products);
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
