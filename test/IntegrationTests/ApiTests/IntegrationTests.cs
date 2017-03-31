using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Shevastream.Services.Factories;
using Moq;
using Shevastream.Tests.Mock;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Shevastream.Tests.IntegrationTests
{
	/// <summary>
	/// Integration tests for ApiController
	/// Does NOT check data layer (eq. if data gets properly stored), there are different tests which check that
	/// </summary>
	public partial class IntegrationTests
	{
		/// <summary>
		/// Server object which mimics a real running server
		/// </summary>
		private readonly TestServer _server;
		/// <summary>
		/// Client object which mimics a real client
		/// </summary>
		private readonly HttpClient _client;

		private readonly ResponseHandler _responseHandler = new ResponseHandler();

		/// <summary>
		/// Setup mock server and client
		/// </summary>
		public IntegrationTests()
		{
			var mockHttpFactory = new Mock<IHttpClientFactory>();
			mockHttpFactory
				.Setup(factory => factory.BuildClient())
				.Returns(new HttpClient(_responseHandler));

			_server = new TestServer(
				new WebHostBuilder()
					.UseEnvironment("Testing")
					.UseStartup<Startup>()
					.ConfigureServices(services =>
					{
						services.Replace(ServiceDescriptor.Singleton<IHttpClientFactory>(mockHttpFactory.Object));
					})
			);

			_client = _server.CreateClient();
		}
	}

	/// <summary>
	/// Set of Http methods needed for testing
	/// </summary>
	public enum HttpMethods
	{
		GET, POST, PUT, DELETE, HEAD
	}
}
