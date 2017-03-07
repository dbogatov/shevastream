using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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

			return Ok("Feedback recorded");
		}
	}
}