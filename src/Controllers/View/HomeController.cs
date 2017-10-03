using System.Threading.Tasks;
using Shevastream.Services;
using Microsoft.AspNetCore.Mvc;
using Shevastream.Extensions;
using Microsoft.Extensions.Configuration;
using System;

namespace Shevastream.Controllers.View
{
	/// <summary>
	/// Controller responsible for home endpoints - /home
	/// </summary>
	public class HomeController : Controller
	{
		private readonly ISiteMapService _siteMap;
		private readonly IBlogService _blogService;

		private readonly int PRIVACY_ID = -1; // Set by config

		public HomeController(ISiteMapService siteMap, IBlogService blogService, IConfiguration config)
		{
			_siteMap = siteMap;
			_blogService = blogService;
			PRIVACY_ID = Convert.ToInt32(config["Data:PrivacyPolicy:Id"]);
		}

		/// <summary>
		/// Generates SiteMap from existing static pages, blog posts and products
		/// </summary>
		public SiteMapResult SiteMap()
		{
			var siteMap = _siteMap.GetSiteMap();
			return new SiteMapResult(siteMap);
		}

		public async Task<IActionResult> Index()
		{
			return View(await _blogService.GetLatestPostsAsync(3));
		}

		public IActionResult FAQ()
		{
			return View();
		}

		public IActionResult Contact()
		{
			return View();
		}

		public IActionResult Profile()
		{
			return View();
		}

		/// <summary>
		/// Redirects to the blog post with privacy statement
		/// </summary>
		public IActionResult Privacy()
		{
			return RedirectToRoutePermanent("Blog", new { id = PRIVACY_ID });
		}
	}
}
