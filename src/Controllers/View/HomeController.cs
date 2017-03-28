using System.Threading.Tasks;
using Shevastream.Services;
using Microsoft.AspNetCore.Mvc;
using Shevastream.Extensions;
using System.Net;

namespace Shevastream.Controllers.View
{
    public class HomeController : Controller
    {
        private readonly ISiteMapService _siteMap;
        private readonly IBlogService _blogService;

        private readonly int PRIVACY_ID = 29;

        public HomeController(ISiteMapService siteMap, IBlogService blogService) 
        {
            _siteMap = siteMap;
            _blogService = blogService;
        }

        public SiteMapResult SiteMap()
        {
            var siteMap = _siteMap.GetSiteMap();
            return new SiteMapResult(siteMap);
        }

        public async Task<IActionResult> Index()
        {	
            var result = View(await _blogService.GetLatestPostsAsync(3));
			result.StatusCode = HttpStatusCode.OK.AsInt();
			return result;
        }

        public IActionResult FAQ()
        {
            var result = View();
			result.StatusCode = HttpStatusCode.OK.AsInt();
			return result;
        }

        public IActionResult Contact()
        {
            var result = View();
			result.StatusCode = HttpStatusCode.OK.AsInt();
			return result;
        }

        public IActionResult Profile()
        {
            var result = View();
			result.StatusCode = HttpStatusCode.OK.AsInt();
			return result;
        }

		public IActionResult Privacy()
        {
            return RedirectToRoutePermanent("Blog", new { id = PRIVACY_ID });
        }
    }
}
