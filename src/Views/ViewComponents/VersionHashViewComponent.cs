using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Shevastream.Extensions;

namespace Shevastream.Views.ViewComponents
{
	public class VersionHashViewComponent : ViewComponent
	{
		private readonly IConfiguration _config;

		public VersionHashViewComponent(IConfiguration config)
		{
			_config = config;
		}

		public async Task<IViewComponentResult> InvokeAsync()
		{
			// hack
			await Task.FromResult(0);
			return View((object)(_config["Version:GitHash"] ?? "none").Truncate(8));
		}
	}
}
