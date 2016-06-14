using EShop.Middleware;
using Microsoft.AspNetCore.Builder;

namespace EShop.Extensions
{
	public static class ToLowercaseExtensions
	{
		public static IApplicationBuilder UseToLowercase(this IApplicationBuilder builder)
		{
			return builder.UseMiddleware<ToLowercaseMiddleware>();
		}
	}
}