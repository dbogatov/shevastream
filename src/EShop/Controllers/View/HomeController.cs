using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace EShop.Controllers
{
	public class HomeController : Controller
	{
		private readonly DataContext _context;
		
		public HomeController(DataContext context) {
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

		public IActionResult Detail()
		{

			return View();
		}

		public IActionResult Profile()
		{

			return View();
		}

		public IActionResult Order()
		{

			return View();
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

			return View();
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
