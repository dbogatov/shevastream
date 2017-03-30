using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Shevastream.ViewModels.Store;
using Xunit;

namespace Shevastream.Tests.IntegrationTests
{
	public partial class IntegrationTests
	{
		[Fact]
		/// <summary>
		/// Check if ApiController.CpuLoad endpoint returns status codes as per documentation
		/// </summary>
		public async Task OrderEndpoint()
		{
			// Arrange
			var requestMade = false;

			_responseHandler.AddHandler(
				new Uri("https://push.dbogatov.org/api/push/shevastream/order"),
				() => requestMade = true
			);

			var _url = "/store/order";

			var parameters = new Dictionary<string, string> { };

			parameters["CustomerName"] = "Test Customer";
			parameters["CustomerEmail"] = "name@example.com";
			parameters["CustomerPhone"] = "8587744556";
			parameters["PaymentMethod"] = "By Card";
			parameters["ShipmentMethod"] = "On Campus";

			var orderCart = new CartViewModel
			{
				Elements = new List<CartElementViewModel> {
					new CartElementViewModel {
						ProductId = 1,
						Quantity = 1
					}
				}
			};

			_client
				.DefaultRequestHeaders
				.Add(
					"Cookie", 
					$"Cart={Uri.EscapeDataString(new Cookie("Cart", JsonConvert.SerializeObject(orderCart)).Value)}"
				);


			// Act
			var redirect = await _client.PostAsync(_url, new FormUrlEncodedContent(parameters));

			// Assert
			Assert.Equal(HttpStatusCode.Redirect, redirect.StatusCode);
			Assert.Contains("thankyou", redirect.Headers.Location.OriginalString);

			Assert.True(requestMade);

			// Clean up
			_responseHandler.RemoveHandler(new Uri("https://push.dbogatov.org/api/push/shevastream/order"));
		}
	}
}
