using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using Shevastream.Models.Entities;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Shevastream.Services.Factories;
using Shevastream.ViewModels;

namespace Shevastream.Services
{
	public interface IPushService
	{
		Task SendOrderAsync(string orderDescription, string name, string email, IEnumerable<Product> products);
		Task SendCallbackAsync(string phone);
		Task SendFeedbackAsync(Feedback feedback);
		Task SendLogAsync(LogMessageViewModel logMessage);
	}

	public class PushService : IPushService
	{
		private readonly IHttpClientFactory _httpClientFactory;

		private readonly string _orderUrl = "https://push.dbogatov.org/api/push/shevastream/order";
		private readonly string _feedbackUrl = "https://push.dbogatov.org/api/push/shevastream/feedback";
		private readonly string _callbackUrl = "https://push.dbogatov.org/api/push/shevastream/callback";

		public PushService(IHttpClientFactory httpClientFactory)
		{
			_httpClientFactory = httpClientFactory;
		}

		public async Task SendOrderAsync(string orderDescription, string name, string email, IEnumerable<Product> products)
		{
			using (var client = _httpClientFactory.BuildClient())
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

		public async Task SendCallbackAsync(string phone)
		{
			using (var client = _httpClientFactory.BuildClient())
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
			using (var client = _httpClientFactory.BuildClient())
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

		// TODO
		public Task SendLogAsync(LogMessageViewModel logMessage)
		{
			return Task.CompletedTask;
		}
	}
}

