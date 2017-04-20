using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Shevastream.Extensions;
using Shevastream.ViewModels.Store;

namespace Shevastream.Controllers
{
	public partial class ApiController
	{
		[HttpPost]
		/// <summary>
		/// Records a callback request.
		/// </summary>
		/// <param name="model">Set of parameters representing data regarding callback</param>
		/// <returns>400 if parameters are malformed, 200 otherwise</returns>
		public async Task<IActionResult> CallbackRequest(CallMeBackViewModel model)
		{
			if (ModelState.IsValid)
			{
				await _push.SendCallbackAsync(model.Phone);

				_logger.LogInformation(LoggingEvents.Feedback.AsInt(), $"Callback request for number {model.Phone}");

				return Ok("Callback request received");
			}
			else
			{
				return BadRequest("Parameters malformed");
			}

		}
	}
}
