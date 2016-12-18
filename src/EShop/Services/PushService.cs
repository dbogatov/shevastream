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
		Task SendConfirmationEmailAsync(string name, string email, IEnumerable<Product> products);

		bool SendAll(string message);
		bool SendTo(IEnumerable<int> to, string message);
	}

	public class PushService : IPushService
	{
		private readonly DataContext _context;
		private readonly string _url = "http://push.dbogatov.org/api/push/send";
		private readonly string _confiramtionUrl = "https://push.dbogatov.org/api/push/shevastream/confirm";

		public PushService(DataContext context)
		{
			_context = context;
		}

		public bool SendAll(string message)
		{
			return SendTo(_context.PushPairs.Select(pp => pp.UserId), message);
		}

		public async Task SendConfirmationEmailAsync(string name, string email, IEnumerable<Product> products)
		{
			using (var client = new HttpClient())
			{
				var values = new Dictionary<string, string>
				{
					{ "name", name },
					{"recipient", email},
					{
						"products",
						JsonConvert.SerializeObject(products.Select(prod => new {
							imageUrl = "https://shevastream.com" + ((JArray)JsonConvert.DeserializeObject(prod.ImageUrls)).ToObject<string[]>()[0],
							description = prod.Description
						}))
					}
				};

				var content = new FormUrlEncodedContent(values);

				await client.PostAsync(_confiramtionUrl, content);
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

				//Console.WriteLine($"{message} : {String.Join(",", tokens)}");

				var response = client.PostAsync(_url, content);
			}

			return true;
		}
	}
}

