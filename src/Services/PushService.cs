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
		/// <summary>
		/// Sends a POST request to the Push service about new order
		/// </summary>
		/// <param name="orderDescription">Short description of the order</param>
		/// <param name="name">Name of the customer</param>
		/// <param name="email">Email of the customer</param>
		/// <param name="products">Collection of prodcuts in the order</param>
		Task SendOrderAsync(string orderDescription, string name, string email, IEnumerable<Product> products);

		/// <summary>
		/// Sends a POST request to the Push service about new callback request
		/// </summary>
		/// <param name="phone">Phone number for which callback is requested</param>
		Task SendCallbackAsync(string phone);

		/// <summary>
		/// Sends a POST request to the Push service about new feedback
		/// </summary>
		/// <param name="feedback">Feedback model to send</param>
		Task SendFeedbackAsync(Feedback feedback);

		/// <summary>
		/// Sends a POST request to the Status server with a log message
		/// </summary>
		/// <param name="logMessage">Log message to send</param>
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
			await SendPostRequestAsync(
				new Dictionary<string, string>
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
				},
				_orderUrl
			);
		}

		public async Task SendCallbackAsync(string phone)
		{
			await SendPostRequestAsync(
				new Dictionary<string, string>
				{
					{ "subscriberPhone", phone }
				},
				_callbackUrl
			);
		}

		public async Task SendFeedbackAsync(Feedback feedback)
		{
			await SendPostRequestAsync(
				new Dictionary<string, string>
				{
					{ "subscriberName", feedback.Name },
					{ "subscriberEmail", feedback.Email },
					{ "subject", feedback.Subject },
					{ "message", feedback.Body }
				},
				_feedbackUrl
			);
		}

		// TODO
		public Task SendLogAsync(LogMessageViewModel logMessage)
		{
			return Task.CompletedTask;
		}

		private async Task SendPostRequestAsync(Dictionary<string, string> parameters, string url)
		{
			using (var client = _httpClientFactory.BuildClient())
			{
				var content = new FormUrlEncodedContent(parameters);

				await client.PostAsync(url, content);
			}
		}
	}
}

