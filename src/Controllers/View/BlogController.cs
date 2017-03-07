using System.Threading;
using System.Threading.Tasks;
using EShop.Services;
using EShop.ViewModels.Blog;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EShop.Controllers.View
{
    public class BlogController : Controller
	{
		private readonly DataContext _context;
		private readonly IBlogService _blog;

		public BlogController(DataContext context, IBlogService blog)
		{
			_context = context;
			_blog = blog;
		}

		public IActionResult Index()
		{
			var model = _blog.GetAllPosts();

			return View(model);
		}

		[HttpPost]
		[Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Publish(
			[FromServices] IBlogService order,
            [FromForm] BlogPostViewModel model,
            CancellationToken requestAborted)
		{
			if (!ModelState.IsValid)
            {
                return View(model);
            }

			var updated = await _blog.UpdatePostAsync(model);
			if (updated == null)
			{
				updated = await _blog.CreatePostAsync(model);
			}

			if (updated.Active)
			{
				return RedirectToRoute("Blog", new { title = updated.TitleUrl, id = updated.Id });
			} else
			{
				return RedirectToAction("Index");
			}
		} 

		public async Task<IActionResult> Post(int id, string title)
		{
			var model = await _blog.GetPostByIdAsync(id);

			if (model != null)
			{
				if (title == null && title != model.TitleUrl)
				{
					return RedirectToRoutePermanent("Blog", new { id = id, title = model.TitleUrl });
				}

				await _blog.AddViewAsync(model);
				return View(model);
			}
			else
			{
				return NotFound();
			}
		}

		[Authorize]
		[Route("Blog/Edit/{id}/{title?}")]
		public async Task<IActionResult> Edit(int id, string title)
		{
			if (title != null)
			{
				ModelState.Clear();

				var post = await _blog.GetPostByIdAsync(id, false);

				if (post != null)
				{
					if (title != post.TitleUrl)
					{
						return RedirectToActionPermanent("Edit", "Blog", new { id = id, title = post.TitleUrl });
					}

					return View(post);	
				}
			}

			return View(new BlogPostViewModel {
				Title = "New post",
				Content = "content here...",
				Id = -1,
				TitleUrl = ""
			});

		}
	}
}
