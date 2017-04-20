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
		/// <summary>
		/// Records a feedback / contact request.
		/// </summary>
		/// <param name="model">Set of parameters representing data regarding the feedback</param>
		/// <returns>400 if parameters are malformed, 200 otherwise</returns>
		public async Task<IActionResult> Feedback(Feedback feedback)
		{
			if (ModelState.IsValid)
			{
				await _push.SendFeedbackAsync(feedback);

				_context.Feedbacks.Add(feedback);
				await _context.SaveChangesAsync();

				_logger.LogInformation(LoggingEvents.Feedback.AsInt(), $"Feedback {feedback} received");

				return Ok("Feedback recorded");
			}
			else
			{
				return BadRequest("Parameters malformed");
			}
		}
	}
}
