using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Shevastream.Controllers
{
    public partial class ApiController
	{
		[HttpGet]
		public async Task<IActionResult> GetOrders()
		{
			return Json(await _orders.GetOrdersAsync());
		}
	}
}