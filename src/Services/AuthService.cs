using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Shevastream.Models;
using Shevastream.Models.Entities;

namespace Shevastream.Services
{
	public interface IAuthService
	{
		Task<User> GetCurrentUser();
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

		public async Task<User> GetCurrentUser()
		{
			if (_http.HttpContext.User.Claims.Any(c => c.Type == "UserId"))
			{
				var userId = Convert.ToInt32(_http.HttpContext.User.Claims.First(c => c.Type == "UserId").Value);
				return await _context.Users.FindAsync(userId);
			}

			return null;
		}
	}
}
