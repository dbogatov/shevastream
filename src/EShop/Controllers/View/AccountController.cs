using Microsoft.AspNetCore.Mvc;

namespace EShop.Controllers
{
	public class AccountController : Controller
	{
		public IActionResult Denied()
		{
			return View();
		}

		public IActionResult Error()
		{
			return View();
		}
	}
}
