using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Shevastream.ActionFilters.ReCaptcha;
using Shevastream.Services;
using Shevastream.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Shevastream.Models;

namespace Shevastream.Controllers.View
{
    public class AccountController : Controller
	{
		private readonly ICryptoService _crypto;
		private readonly DataContext _context;

		public AccountController(ICryptoService crypto, DataContext context)
		{
			_crypto = crypto;
			_context = context;
		}

		public IActionResult Login()
		{
			return View(new ReturnUrlViewModel
			{
				ReturnUrl = Request.Query["returnurl"].FirstOrDefault() ?? "",
				IsError = false
			});
		}

		public IActionResult Logout()
		{
			HttpContext.Authentication.SignOutAsync("MyCookieMiddlewareInstance");
			return RedirectToAction("Index", "Home");
		}

		[ReCaptcha]
		public IActionResult Authenticate()
		{
			if (!ModelState.IsValid)
			{
				return View(
					"Login",
					new ReturnUrlViewModel
					{
						ReturnUrl = Request.Query["returnurl"],
						IsError = true,
						Error = "Wrong reCAPTCHA"
					}
				);
			}

			string hash = _crypto.CalculateHash(Request.Form["password"]);
			var user = _context.Users.FirstOrDefault(u => u.PassHash == hash);

			if (user != null)
			{
				var principal = new ClaimsPrincipal(
					new ClaimsIdentity(
						new List<Claim> {
							new Claim("UserId", user.Id.ToString()),
						}, "API User"
					)
				);

				HttpContext.Authentication.SignInAsync("MyCookieMiddlewareInstance", principal);

				if (string.IsNullOrEmpty(Request.Query["returnurl"]))
				{
					return RedirectToAction("Index", "Store"); 
				} else
				{
					return Redirect(Request.Query["returnurl"]);	
				}
			}
			else
			{
				return View("Login", new ReturnUrlViewModel
				{
					ReturnUrl = Request.Query["returnurl"],
					IsError = true,
					Error = "Wrong password"
				});
			}
		}
	}
}
