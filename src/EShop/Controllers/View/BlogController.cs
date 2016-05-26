using System.Linq;
using EShop.Services;
using EShop.ViewModels.Blog;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
			var model = _context
				.BlogPosts
				.Include(bp => bp.Author)
				.Where(bp => bp.Active)
				.Select(bp => new BlogPostViewModel
				{
					Id = bp.Id,
					AuthorName = bp.Author.NickName,
					DatePosted = bp.DatePosted,
					Title = bp.Title,
					TitleUrl = bp.TitleUrl
				});
			return View(model);
		}

		public IActionResult Post(string title)
		{
			if (_context.BlogPosts.Any(bp => bp.Active && bp.TitleUrl == title))
			{
				var post =
					BlogPostViewModel.FromBlogPost(_context
					.BlogPosts.Include(bp => bp.Author)
					.First(
						bp =>
							bp.Active &&
							bp.TitleUrl == title
					));

				post.HtmlContent = _blog.MarkDownToHtml(post.Content);
				post.AuthorName = post.Author.NickName;
				post.Author = null;

				return View(post);
			}
			else
			{
				return NotFound();
			}
		}

		[Authorize]
		[Route("Blog/Edit/{title}")]
		public IActionResult Edit(string title)
		{
			if (_context.BlogPosts.Any(bp => bp.Active && bp.TitleUrl == title))
			{
				var post =
					BlogPostViewModel.FromBlogPost(_context
					.BlogPosts.Include(bp => bp.Author)
					.First(
						bp =>
							bp.Active &&
							bp.TitleUrl == title
					));

				post.HtmlContent = _blog.MarkDownToHtml(post.Content);

				return View(post);
			}
			else
			{
				return View("New");
			}
		}
	}
}
