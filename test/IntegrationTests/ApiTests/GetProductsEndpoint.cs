using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using Xunit;

namespace Shevastream.Tests.IntegrationTests
{
	public partial class IntegrationTests
	{
		[Fact]
		public async Task GetProductsEndpoint()
		{
			// Arrange
			var _url = "/api/getproducts";

			var parameters = new Dictionary<string, string> { };

			// Act
			var ok = await _client.GetAsync(QueryHelpers.AddQueryString(_url, parameters));

			// Check OK body (should be parsable JSON)
			var data = JsonConvert.DeserializeObject(
				await ok.Content.ReadAsStringAsync()
			);

			// Assert
			Assert.Equal(HttpStatusCode.OK, ok.StatusCode);

			Assert.NotNull(data);
		}
	}
}
