using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Shevastream.Tests.Mock
{
	public class ResponseHandler : DelegatingHandler
	{
		private readonly Dictionary<Uri, Action> _actions = new Dictionary<Uri, Action>();

		public void AddHandler(Uri uri, Action action)
		{
			_actions.Add(uri, action);
		}

		public void RemoveHandler(Uri uri)
		{
			_actions.Remove(uri);
		}

		protected async override Task<HttpResponseMessage> SendAsync(
			HttpRequestMessage request,
			CancellationToken cancellationToken
		)
		{
			if (_actions.ContainsKey(request.RequestUri))
			{
				_actions[request.RequestUri].Invoke();
				return await Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
			}
			else
			{
				return await Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound));
			}
		}
	}

}
