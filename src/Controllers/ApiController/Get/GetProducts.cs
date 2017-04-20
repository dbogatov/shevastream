using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Shevastream.Controllers
{
	public partial class ApiController
	{
		[HttpGet]
		/// <summary>
		/// Returns a list of products.
		/// </summary>
		/// <returns>List of products</returns>
		public async Task<IActionResult> GetProducts()
		{
			return Json(await _context.Products.ToListAsync());
		}
	}
}
