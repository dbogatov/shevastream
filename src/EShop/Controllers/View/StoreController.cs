using System.Collections.Generic;
using System.Linq;
using EShop.Controllers.API;
using EShop.ViewModels.Store;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EShop.Controllers.View
{
	public class StoreController : Controller
	{
		private readonly string CART_COOKIE_NAME = "Cart";

		private readonly DataContext _context;
		private readonly HttpContext _http;

		public StoreController(DataContext context, IHttpContextAccessor http)
		{
			_context = context;
			_http = http.HttpContext;
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

		public IActionResult Cart()
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
