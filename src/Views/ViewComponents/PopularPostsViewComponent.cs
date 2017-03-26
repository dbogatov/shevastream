using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shevastream.Models;

namespace Shevastream.Views.ViewComponents
{
    public class PopularPostsViewComponent : ViewComponent
    {
        private readonly DataContext _context;

        public PopularPostsViewComponent(DataContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(int numPosts)
        {
            var posts = await _context.BlogPosts.Where(post => post.Active).OrderByDescending(post => post.Views).Take(numPosts).ToListAsync();
            return View(posts);
        }
    }
}