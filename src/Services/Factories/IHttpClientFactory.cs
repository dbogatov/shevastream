using System.Net.Http;

namespace Shevastream.Services.Factories
{
	public interface IHttpClientFactory
	{
		/// <summary>
		/// Returns HttpClient
		/// </summary>
		HttpClient BuildClient();
	}

	public class HttpClientFactory : IHttpClientFactory
	{
		public HttpClient BuildClient()
		{
			return new HttpClient();
		}
	}
}
