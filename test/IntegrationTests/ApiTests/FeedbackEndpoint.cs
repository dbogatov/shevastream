using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Shevastream.Tests.IntegrationTests
{
	public partial class IntegrationTests
	{
		[Fact]
		/// <summary>
		/// Check if ApiController.CpuLoad endpoint returns status codes as per documentation
		/// </summary>
		public async Task FeedbackEndpoint()
		{
			// Arrange
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

			// Act
			var ok = await _client.PostAsync(_url, new FormUrlEncodedContent(parameters));

			// Assert
			Assert.Equal(HttpStatusCode.OK, ok.StatusCode);
			
			Assert.True(requestMade);

			// Clean up
			_responseHandler.RemoveHandler(new Uri("https://push.dbogatov.org/api/push/shevastream/feedback"));
		}
	}
}
