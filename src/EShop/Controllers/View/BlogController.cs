﻿using EShop.Services;
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

		public IActionResult Post(string title)
		{
			var model = _blog.GetPostByTitle(title);

			if (model != null)
			{
				return View(model);
			}
			else
			{
				return NotFound();
			}
		}

		[Authorize]
		[Route("Blog/Edit/{title?}")]
		public IActionResult Edit(string title)
		{
			if (title != null)
			{
				var post = _blog.GetPostByTitle(title, false);

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
