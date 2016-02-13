using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EShop.Models.Enitites;
using EShop.Services;
using EShop.ViewModels.Home;
using Microsoft.AspNet.Mvc;
using Newtonsoft.Json;

namespace EShop.Controllers.API {

	[Produces("application/json")]
	[Route("api/Feedback")]
	public class FeedbackController : Controller {
		private readonly ITelegramSender _telegram;

		public FeedbackController(ITelegramSender telegram) {
			_telegram = telegram;
		}

		// POST api/feedback
		[HttpPost]
		public bool Post(Feedback feedback) {
			_telegram.SendMessageAsync(feedback.ToString());

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