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
		public async Task<IActionResult> CallbackRequest(CallMeBackViewModel model)
		{
			await _push.SendCallbackAsync(model.Phone);

			_logger.LogInformation(LoggingEvents.Feedback.AsInt(), $"Callback request for number {model.Phone}");

			return Ok("Callback request received");
		}
	}
}
