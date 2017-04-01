using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Shevastream.Extensions;
using Shevastream.Models.Entities;

namespace Shevastream.Controllers
{
	public partial class ApiController
	{
		[HttpPost]
		public async Task<IActionResult> Feedback(Feedback feedback)
		{
			await _push.SendFeedbackAsync(feedback);

			_context.Feedbacks.Add(feedback);
			await _context.SaveChangesAsync();

			_logger.LogInformation(LoggingEvents.Feedback.AsInt(), $"Feedback {feedback} received");

			return Ok("Feedback recorded");
		}
	}
}
