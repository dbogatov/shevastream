using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using EShop.Services;
using EShop.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EShop.Controllers.View
{
	public class AccountController : Controller
	{
		private readonly string CAPTCHA_URL = "https://www.google.com/recaptcha/api/siteverify";
		private readonly string SECRET = "6LfYAiETAAAAAMGrNgFMnY5rnc5jxjFuU8yveqnj";

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

		public IActionResult Authenticate()
		{
			// Get recaptcha value
			var captchaResponse = Request.Form["g-recaptcha-response"];

			using (var client = new HttpClient())
			{
				var values = new Dictionary<string, string>
				{
					{ "secret", SECRET },
					{ "response", captchaResponse },
					{ "remoteip", Request.HttpContext.Connection.RemoteIpAddress.ToString() }
				};


				var content = new FormUrlEncodedContent(values);

				var response = client.PostAsync(CAPTCHA_URL, content).Result;

				if (response.IsSuccessStatusCode)
				{
					string responseString = response.Content.ReadAsStringAsync().Result;

					var captchaResult = JsonConvert.DeserializeObject<CaptchaResponseViewModel>(responseString);

					if (captchaResult.Success)
					{
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

							return Redirect(Request.Query["returnurl"]);
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
					else
					{
						return View("Login", new ReturnUrlViewModel
						{
							ReturnUrl = Request.Query["returnurl"],
							IsError = true,
							Error = "Wrong reCAPTCHA"
						});
					}
				}
				else
				{
					return View("Login", new ReturnUrlViewModel
					{
						ReturnUrl = Request.Query["returnurl"],
						IsError = true,
						Error = "Unknown error"
					});
				}
			}
		}
	}
}
