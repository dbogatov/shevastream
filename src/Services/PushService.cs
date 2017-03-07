using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using EShop.Models.Enitites;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace EShop.Services
{
	public interface IPushService
	{
		Task SendOrderAsync(string orderDescription, string name, string email, IEnumerable<Product> products);

		Task SendCallbackAsync(string phone);

		Task SendFeedbackAsync(Feedback feedback);

		bool SendAll(string message);
		bool SendTo(IEnumerable<int> to, string message);
	}

	public class PushService : IPushService
	{
		private readonly DataContext _context;
		private readonly string _url = "http://push.dbogatov.org/api/push/send";
		private readonly string _orderUrl = "https://push.dbogatov.org/api/push/shevastream/order";
		private readonly string _feedbackUrl = "https://push.dbogatov.org/api/push/shevastream/feedback";
		private readonly string _callbackUrl = "https://push.dbogatov.org/api/push/shevastream/callback";

		public PushService(DataContext context)
		{
			_context = context;
		}

		public bool SendAll(string message)
		{
			return SendTo(_context.PushPairs.Select(pp => pp.UserId), message);
		}

		public async Task SendOrderAsync(string orderDescription, string name, string email, IEnumerable<Product> products)
		{
			using (var client = new HttpClient())
			{
				var values = new Dictionary<string, string>
				{
					{ "order", orderDescription },
					{ "name", name },
					{ "recipient", email},
					{
						"products",
						JsonConvert.SerializeObject(products.Select(prod => new {
							imageUrl = "https://shevastream.com" + ((JArray)JsonConvert.DeserializeObject(prod.ImageUrls)).ToObject<string[]>()[0],
							description = prod.Description
						}))
					}
				};

				var content = new FormUrlEncodedContent(values);

				await client.PostAsync(_orderUrl, content);
			}
		}

		public bool SendTo(IEnumerable<int> to, string message)
		{
			var tokens =
				from pushPair in _context.PushPairs
				join userId in to on pushPair.UserId equals userId
				select pushPair.DeviceToken;

			using (var client = new HttpClient())
			{
				var values = new Dictionary<string, string>
				{
					{ "appname", "shevastream" },
					{ "message", message },
					{ "tokens", String.Join(",", tokens) },
					{ "production", "false" }
				};


				var content = new FormUrlEncodedContent(values);

				var response = client.PostAsync(_url, content);
			}

			return true;
		}

		public async Task SendCallbackAsync(string phone)
		{
			using (var client = new HttpClient())
			{
				var values = new Dictionary<string, string>
				{
					{ "subscriberPhone", phone }
				};

				var content = new FormUrlEncodedContent(values);

				await client.PostAsync(_callbackUrl, content);
			}
		}

		public async Task SendFeedbackAsync(Feedback feedback)
		{
			using (var client = new HttpClient())
			{
				var values = new Dictionary<string, string>
				{
					{ "subscriberName", feedback.Name },
					{ "subscriberEmail", feedback.Email },
					{ "subject", feedback.Subject },
					{ "message", feedback.Body }
				};

				var content = new FormUrlEncodedContent(values);

				await client.PostAsync(_feedbackUrl, content);
			}
		}
	}
}

