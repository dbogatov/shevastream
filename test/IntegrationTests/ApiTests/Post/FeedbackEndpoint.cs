using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Shevastream.Tests.IntegrationTests
{
	public partial class ApiControllerTest
	{
		[Fact]
		/// <summary>
		/// Check if ApiController.CpuLoad endpoint returns status codes as per documentation
		/// </summary>
		public async Task FeedbackEndpoint()
		{
			// ARRANGE

			var requestMade = false;
			
			_responseHandler.AddHandler(
				new Uri("https://push.dbogatov.org/api/push/shevastream/feedback"), 
				() => requestMade = true
			);

			var _url = "/api/feedback";

			var parameters = new Dictionary<string, string> { };

			// Check OK when metric already exists
			parameters["email"] = "name@example.com";
			parameters["subject"] = "Test";
			parameters["body"] = "Test body";
			parameters["name"] = "Test customer";

			// ACT

			var ok = await _client.PostAsync(_url, new FormUrlEncodedContent(parameters));

			// ASSERT

			Assert.Equal(HttpStatusCode.OK, ok.StatusCode);
			
			Assert.True(requestMade);
		}
	}
}
