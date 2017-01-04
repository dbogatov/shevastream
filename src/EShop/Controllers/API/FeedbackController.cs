using EShop.Models.Enitites;
using EShop.Services;
using EShop.ViewModels.Store;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace EShop.Controllers.API
{

	[Produces("application/json")]
	[Route("api/Feedback")]
	public class FeedbackController : Controller
	{
		private readonly IPushService _push;
		private readonly DataContext _context;


		public FeedbackController(IPushService push, DataContext context, IHostingEnvironment env)
		{
			_push = push;
			_context = context;
		}

		// POST api/feedback
		[HttpPost]
		public bool Post(Feedback feedback)
		{
			_push.SendFeedbackAsync(feedback);

			_context.Feedbacks.Add(feedback);
			_context.SaveChanges();

			return true;
		}

		// POST api/feedback/callMeBack
		[Route("CallMeBack")]
		[HttpPost]
		public bool CallMeBack(CallMeBackViewModel request)
		{
			_push.SendCallbackAsync(request.Phone);

			return true;
		}
	}
}