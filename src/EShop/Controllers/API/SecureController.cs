using System.Collections.Generic;
using System.Linq;
using EShop.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EShop.Controllers.API
{

	[Produces("application/json")]
	[Route("api/Secure")]
	public class SecureController : Controller
	{
		private readonly DataContext _context;
		private readonly ICryptoService _crypto;

		public SecureController(
			DataContext context,
			ICryptoService crypto
		)
		{
			_context = context;
			_crypto = crypto;
		}

		// GET: api/Secure/Auth
		[Route("Auth/{code}")]
		[HttpGet]
		public IActionResult Authorize(string code)
		{

			string hash = _crypto.CalculateHash(code);
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
				
				return new ObjectResult(new {
					Name = user.NickName,
					Password = code,
					Id = user.Id,
					ImageUrl = user.ImageUrl
				});
			}

			return new ObjectResult(false);
		}

		// GET: api/Secure/Logout
		[Route("Logout")]
		[HttpGet]
		public IActionResult Logout()
		{
			HttpContext.Authentication.SignOutAsync("MyCookieMiddlewareInstance");

			return new ObjectResult(true);
		}

	}
}