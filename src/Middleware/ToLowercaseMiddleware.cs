using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Shevastream.Extensions;

namespace Shevastream.Middleware
{
	public class ToLowercaseMiddleware
	{
		private readonly RequestDelegate _next;

		public ToLowercaseMiddleware(RequestDelegate next)
		{
			_next = next;
		}

		public async Task Invoke(HttpContext context)
		{
			//You don't want to redirect on posts, or images/css/js
			bool isGet = context.Request.Method.ToLowerInvariant().Contains("get");
			var absolutePath = UriHelper.GetDisplayUrl(context.Request);
			var isApi = absolutePath.Contains("/api/", StringComparison.OrdinalIgnoreCase);
			var isFile = absolutePath.Contains(".");

			if (isGet && !isFile && !isApi)
			{
				string lowercaseURL = (context.Request.Scheme + "://" + context.Request.Host.Host + (context.Request.Host.Port.HasValue ? $":{context.Request.Host.Port}" : "") + context.Request.Path);
				if (Regex.IsMatch(lowercaseURL, @"[A-Z]"))
				{
					//You don't want to change casing on query strings
					lowercaseURL = lowercaseURL.ToLower() + context.Request.QueryString;

					context.Response.Clear();
					context.Response.StatusCode = 301;
					context.Response.Headers.Add("Location", lowercaseURL);
					return;
				}
			}
			await _next.Invoke(context);
		}
	}
}
