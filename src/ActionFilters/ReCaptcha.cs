using System.Collections.Generic;
using System.Net.Http;
using Shevastream.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Hosting;

namespace Shevastream.ActionFilters
{
	public class ReCaptcha : ActionFilterAttribute
	{
		private readonly string CAPTCHA_URL = "https://www.google.com/recaptcha/api/siteverify";
		private readonly string SECRET = "6LfYAiETAAAAAMGrNgFMnY5rnc5jxjFuU8yveqnj";
		private readonly IHostingEnvironment _env;

		public ReCaptcha(IHostingEnvironment env)
		{
			_env = env;
		}

		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			if (!_env.IsProduction())
			{
				return;
			}

			try
			{
				// Get recaptcha value
				var captchaResponse = filterContext.HttpContext.Request.Form["g-recaptcha-response"];

				using (var client = new HttpClient())
				{
					var values = new Dictionary<string, string>
					{
						{ "secret", SECRET },
						{ "response", captchaResponse },
						{ "remoteip", filterContext.HttpContext.Request.HttpContext.Connection.RemoteIpAddress.ToString() }
					};

					var content = new FormUrlEncodedContent(values);

					var result = client.PostAsync(CAPTCHA_URL, content).Result;

					if (result.IsSuccessStatusCode)
					{
						string responseString = result.Content.ReadAsStringAsync().Result;

						var captchaResult = JsonConvert.DeserializeObject<CaptchaResponseViewModel>(responseString);

						if (!captchaResult.Success)
						{
							((Controller)filterContext.Controller).ModelState.AddModelError("ReCaptcha", "Captcha not solved");
						}
					}
					else
					{
						((Controller)filterContext.Controller).ModelState.AddModelError("ReCaptcha", "Captcha error");
					}
				}

			}
			catch (System.Exception)
			{
				((Controller)filterContext.Controller).ModelState.AddModelError("ReCaptcha", "Unknown error");
			}
		}
	}
}
