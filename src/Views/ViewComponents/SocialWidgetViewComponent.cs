using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Shevastream.Views.ViewComponents
{
    public class SocialWidgetViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(string url)
        {
			// hack
			await Task.FromResult(0);
            return View((object)url);
        }
    }
}