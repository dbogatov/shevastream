using System.Threading.Tasks;
using Shevastream.Services;
using Shevastream.ViewModels.Blog;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Shevastream.Controllers.View
{
	/// <summary>
	/// Controller responsible for blog endpoints - /blog
	/// </summary>
	public class BlogController : Controller
	{
		private readonly IBlogService _blog;

		public BlogController(IBlogService blog)
		{
			_blog = blog;
		}

		public async Task<IActionResult> Index()
		{
			var model = await _blog.GetAllPostsAsync();

			return View(model);
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Publish(BlogPostViewModel model)
		{
			// Verify parameter consistency
			if (!ModelState.IsValid)
			{
				return BadRequest();
			}

			// Try to update, otherwise create
			var updated = await _blog.UpdatePostAsync(model);
			if (updated == null)
			{
				updated = await _blog.CreatePostAsync(model);
			}

			// Show the post or redirect to index
			if (updated.Active)
			{
				return RedirectToRoute("Blog", new { title = updated.TitleUrl, id = updated.Id });
			}
			else
			{
				return RedirectToAction("Index");
			}
		}

		public async Task<IActionResult> Post(int id, string title)
		{
			// Try to retrieve post by id
			var model = await _blog.GetPostByIdAsync(id);

			if (model != null)
			{
				// If found, verify title-url (needed for SEO consistency)
				if (title == null || title != model.TitleUrl)
				{
					// If title-url is wrong, redirect ot the right one
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
			if (id > -1)
			{
				ModelState.Clear();

				var post = await _blog.GetPostByIdAsync(id, false);

				if (post != null)
				{
					return View(post);
				}
				else
				{
					return NotFound();
				}
			}
			else
			{
				return View(new BlogPostViewModel
				{
					Title = "New post",
					Content = "content here...",
					Id = -1,
					TitleUrl = ""
				});
			}
		}
	}
}
