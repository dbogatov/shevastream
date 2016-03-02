using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
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
