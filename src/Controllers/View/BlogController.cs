﻿using System.Threading;
using System.Threading.Tasks;
using Shevastream.Services;
using Shevastream.ViewModels.Blog;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shevastream.Models;
using System.Net;
using Shevastream.Extensions;
using System.Linq;

namespace Shevastream.Controllers.View
{
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

			var result = View(model);
			result.StatusCode = (model.Count() > 0 ? HttpStatusCode.OK : HttpStatusCode.NoContent).AsInt();
			return result;
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Publish(BlogPostViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest();
			}

			var updated = await _blog.UpdatePostAsync(model);
			if (updated == null)
			{
				updated = await _blog.CreatePostAsync(model);
			}

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
