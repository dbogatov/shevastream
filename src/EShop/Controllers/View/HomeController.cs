using System.Threading.Tasks;
using EShop.Models;
using EShop.Services;
using Microsoft.AspNetCore.Mvc;

namespace EShop.Controllers.View
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

		public IActionResult Privacy()
        {
            return RedirectToRoutePermanent("Blog", new { id = PRIVACY_ID });
        }
    }
}
