using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Shevastream.Models;

namespace Shevastream.Services
{
	public interface IAuthService
	{
		/// <summary>
		/// Returns an id of the currently authenticated user
		/// Or null if user is not authenticated
		/// </summary>
		/// <returns>Id of the authenticated user</returns>
		int? GetCurrentUserId();
	}

	public class AuthService : IAuthService
	{
		private readonly IHttpContextAccessor _http;
		private readonly IDataContext _context;

		public AuthService(IHttpContextAccessor http, IDataContext context)
		{
			_http = http;
			_context = context;
		}

		public int? GetCurrentUserId()
		{
			if (_http.HttpContext.User.Claims.Any(c => c.Type == "UserId"))
			{
				return Convert.ToInt32(_http.HttpContext.User.Claims.First(c => c.Type == "UserId").Value);
			}

			return null;
		}
	}
}
