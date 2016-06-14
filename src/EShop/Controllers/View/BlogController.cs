﻿using System.Threading;
using System.Threading.Tasks;
using EShop.Models.Enitites;
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
				return RedirectToRoute("Blog", new { title = updated.TitleUrl });
			} else
			{
				return RedirectToAction("Index");
			}
		} 

		public async Task<IActionResult> Post(string title)
		{
			var model = await _blog.GetPostByTitleAsync(title);

			if (model != null)
			{
				await _blog.AddViewAsync(model);
				return View(model);
			}
			else
			{
				return NotFound();
			}
		}

		[Authorize]
		[Route("Blog/Edit/{title?}")]
		public async Task<IActionResult> Edit(string title)
		{
			if (title != null)
			{
				ModelState.Clear();

				var post = await _blog.GetPostByTitleAsync(title, false);

				if (post != null)
				{
					post.HtmlContent = _blog.MarkDownToHtml(post.Content);

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
