using System.Collections.Generic;
using System.Linq;
using EShop.Models.Enitites;
using Microsoft.AspNetCore.Mvc;

namespace EShop.Controllers.API
{

    [Produces("application/json")]
	[Route("api/Product")]
	public class ProductController : Controller
	{
		private readonly DataContext _context;
		
		public ProductController(DataContext context)
		{
			_context = context;
		}

		// GET: api/product
		[HttpGet]
		public IEnumerable<Product> GetProducts()
		{
			return _context.Products.AsEnumerable();
		}

	}
}