using Shevastream.Middleware;
using Microsoft.AspNetCore.Builder;

namespace Shevastream.Extensions
{
	/// <summary>
	/// Part of the ToLowerCaseMiddleware
	/// </summary>
	public static class ToLowercaseExtensions
	{
		public static IApplicationBuilder UseToLowercase(this IApplicationBuilder builder)
		{
			return builder.UseMiddleware<ToLowercaseMiddleware>();
		}
	}
}
