using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Shevastream.Extensions;
using Shevastream.Models;

namespace Shevastream.Views.ViewComponents
{
    public class VersionHashViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
			// hack
			await Task.FromResult(0);
            return View((object)(DataContext.version ?? "none").Truncate(8));
        }
    }
}
