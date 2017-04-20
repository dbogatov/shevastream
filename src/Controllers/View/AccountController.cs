using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Shevastream.ActionFilters;
using Shevastream.Services;
using Shevastream.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Shevastream.Models;

namespace Shevastream.Controllers.View
{
	/// <summary>
	/// Controller responsible for authentication related endpoints - /account
	/// </summary>
	public class AccountController : Controller
	{
		private readonly ICryptoService _crypto;
		private readonly IDataContext _context;

		public AccountController(ICryptoService crypto, IDataContext context)
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

		[ServiceFilter(typeof(ReCaptcha))]
		public IActionResult Authenticate()
		{
			// Check ReCaptcha
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

			// Compute hash of password and find a user with such hash
			string hash = _crypto.CalculateHash(Request.Form["password"]);
			var user = _context.Users.FirstOrDefault(u => u.PassHash == hash);

			if (user != null)
			{
				// User found, authenticate him
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
					return RedirectToAction("Index", "Home");
				}
				else
				{
					return Redirect(Request.Query["returnurl"]);
				}
			}
			else
			{
				// No user found, wrong password
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
