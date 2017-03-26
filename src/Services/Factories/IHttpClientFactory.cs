using System.Net.Http;

namespace Shevastream.Services.Factories
{
	public interface IHttpClientFactory
	{
		HttpClient BuildClient();
	}

	public class HttpClientFactory : IHttpClientFactory
	{
		public HttpClient BuildClient() {
			return new HttpClient();
		}
	}
}