using Microsoft.AspNet.Mvc;

namespace EShop.Controllers
{
	public class AccountController : Controller
	{
		public IActionResult Unauthorized()
		{
			return View();
		}

		public IActionResult Error()
		{
			return View();
		}
	}
}
