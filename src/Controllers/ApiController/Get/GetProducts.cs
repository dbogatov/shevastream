using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Shevastream.Controllers
{
    public partial class ApiController
	{
		[HttpGet]
		public async Task<IActionResult> GetProducts()
		{
			return Json(await _context.Products.ToListAsync());
		}
	}
}
