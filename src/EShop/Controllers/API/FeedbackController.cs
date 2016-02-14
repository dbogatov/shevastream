using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EShop.Models.Enitites;
using EShop.Services;
using EShop.ViewModels.Home;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Mvc;
using Newtonsoft.Json;

namespace EShop.Controllers.API {

	[Produces("application/json")]
	[Route("api/Feedback")]
	public class FeedbackController : Controller {
		private readonly ITelegramSender _telegram;
		private readonly DataContext _context;
		

		public FeedbackController(ITelegramSender telegram, DataContext context, IHostingEnvironment env) {
			_telegram = telegram;
			_context = context;
		}

		// POST api/feedback
		[HttpPost]
		public bool Post(Feedback feedback) {
			_telegram.SendMessageAsync(feedback.ToString());

			_context.Feedbacks.Add(feedback);
			_context.SaveChanges();

			return true;
		}
		
		// POST api/feedback/callMeBAck
		[Route("CallMeBack")]
		[HttpPost]
		public bool CallMeBack(CallMeBackViewModel request) {
			_telegram.SendMessageAsync(request.ToString());

			return true;
		}
	}
}