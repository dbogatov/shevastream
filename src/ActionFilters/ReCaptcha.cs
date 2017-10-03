using System.Collections.Generic;
using System.Net.Http;
using Shevastream.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Shevastream.Extensions;

namespace Shevastream.ActionFilters
{
	/// <summary>
	/// Action filter responsible for validating ReCaptcha challenge.
	/// Grabs "g-recaptcha-response" from form request and sends it to Google service.
	/// Sets model errors if unsuccessful.
	/// </summary>
	public class ReCaptcha : ActionFilterAttribute
	{
		private readonly string CAPTCHA_URL = "https://www.google.com/recaptcha/api/siteverify";
		private readonly string SECRET = "set-by-config";
		private readonly IConfiguration _conf;
		private readonly ILogger _logger;

		public ReCaptcha(IConfiguration conf, ILogger<ReCaptcha> logger)
		{
			_conf = conf;
			_logger = logger;

			SECRET = _conf["ReCaptcha:SecretKey"];
		}

		public override async Task OnActionExecutionAsync(ActionExecutingContext filterContext, ActionExecutionDelegate next)
		{
			if (!Convert.ToBoolean(_conf["ReCaptcha:Enable"]))
			{
				try
				{
					// Get ReCaptcha value
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

						// Send challenge solution to Google
						var result = await client.PostAsync(CAPTCHA_URL, content);

						if (result.IsSuccessStatusCode)
						{
							string responseString = await result.Content.ReadAsStringAsync();

							var captchaResult = JsonConvert.DeserializeObject<CaptchaResponseViewModel>(responseString);

							if (!captchaResult.Success)
							{
								_logger.LogWarning(LoggingEvents.ActionFilters.AsInt(), "Captcha not solved");

								((Controller)filterContext.Controller).ModelState.AddModelError("ReCaptcha", "Captcha not solved");
							}

							// If here, then passed
						}
						else
						{
							_logger.LogWarning(LoggingEvents.ActionFilters.AsInt(), "Unknown captcha error");

							((Controller)filterContext.Controller).ModelState.AddModelError("ReCaptcha", "Captcha error");
						}
					}

				}
				catch (System.Exception e)
				{
					_logger.LogError(LoggingEvents.ActionFilters.AsInt(), e, "Unknown error");

					((Controller)filterContext.Controller).ModelState.AddModelError("ReCaptcha", "Unknown error");
				}
			}

			// Invoke next stage in the pipeline
			await next();
		}
	}
}
