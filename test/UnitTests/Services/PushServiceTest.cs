using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json;
using Shevastream.Models.Entities;
using Shevastream.Services;
using Shevastream.Services.Factories;
using Shevastream.Tests.Mock;
using Xunit;

namespace Shevastream.Tests.UnitTests.Services
{
	public class PushServiceTest
	{
		private readonly ResponseHandler _responseHandler = new ResponseHandler();
		private readonly Mock<IHttpClientFactory> _httpFactory = new Mock<IHttpClientFactory>();

		public PushServiceTest()
		{
			_httpFactory = new Mock<IHttpClientFactory>();
			_httpFactory
				.Setup(factory => factory.BuildClient())
				.Returns(new HttpClient(_responseHandler));
		}

		[Theory]
		[InlineData(PushTask.Order)]
		[InlineData(PushTask.Callback)]
		[InlineData(PushTask.Feedback)]
		public async Task SendsRequest(PushTask task)
		{
			// Arrange
			var requestMade = false;

			Uri uri = null;

			switch (task)
			{
				case PushTask.Callback:
					uri = new Uri("https://push.dbogatov.org/api/push/shevastream/callback");
					break;
				case PushTask.Feedback:
					uri = new Uri("https://push.dbogatov.org/api/push/shevastream/feedback");
					break;
				case PushTask.Order:
					uri = new Uri("https://push.dbogatov.org/api/push/shevastream/order");
					break;
			}

			_responseHandler.AddHandler(uri, () => requestMade = true);

			var pushService = new PushService(_httpFactory.Object);

			// Act
			switch (task)
			{
				case PushTask.Callback:
					await pushService.SendCallbackAsync("5869985412");
					break;
				case PushTask.Feedback:
					await pushService.SendFeedbackAsync(new Feedback {
						Email = "jdoe@example.com",
						Subject = "Subject",
						Body = "Body",
						Name = "John Doe"
					});
					break;
				case PushTask.Order:
					await pushService.SendOrderAsync(
						"My order",
						"John Doe",
						"jdoe@example.com",
						new List<Product> {
							new Product { 
								ImageUrls =  JsonConvert.SerializeObject(new List<string> { "url1", "url2" }),
								Description = "Description"
							 }
						}
					);
					break;
			}

			// Assert
			Assert.True(requestMade);

			// Clean up
			_responseHandler.RemoveHandler(uri);
		}
	}

	public enum PushTask
	{
		Order, Callback, Feedback
	}
}

