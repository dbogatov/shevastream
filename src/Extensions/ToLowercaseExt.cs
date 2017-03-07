using Shevastream.Middleware;
using Microsoft.AspNetCore.Builder;

namespace Shevastream.Extensions
{
	public static class ToLowercaseExtensions
	{
		public static IApplicationBuilder UseToLowercase(this IApplicationBuilder builder)
		{
			return builder.UseMiddleware<ToLowercaseMiddleware>();
		}
	}
}