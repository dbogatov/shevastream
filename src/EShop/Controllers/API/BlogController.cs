using Microsoft.AspNetCore.Mvc;
using System.Linq;
using EShop.Services;
using EShop.ViewModels.Blog;
using System;
using EShop.Models.Enitites;
using Microsoft.AspNetCore.Authorization;

namespace EShop.Controllers.API
{

    [Produces("application/json")]
	[Route("api/Blog")]
	public class BlogController : Controller
	{
        private readonly IBlogService _blog;
        private readonly DataContext _context;

        public BlogController(IBlogService blog, DataContext context)
		{
            _blog = blog;
            _context = context;
        }

		// POST api/Blog
		[HttpPost]
		[Authorize]
		public void Post(BlogPostViewModel post)
		{
			if (_context.BlogPosts.Any(bp => bp.Id == post.Id))
			{
				var userId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "UserId").Value);
				
                var old = _context.BlogPosts.First(bp => bp.Id == post.Id);
                old.Title = post.Title.Trim();
                old.TitleUrl = _blog.GenerateUrlFromTitle(post.Title);
                old.Content = post.Content;
                old.DateUpdated = DateTime.Now;
                old.AuthorId = userId;

                _context.SaveChanges();
            }
		}
		
		// POST api/Blog/Activate
		[HttpPost]
		[Authorize]
		[Route("Activate")]
		public void Activate(BlogPostViewModel post)
		{
			if (_context.BlogPosts.Any(bp => bp.Id == post.Id))
			{
                var old = _context.BlogPosts.First(bp => bp.Id == post.Id);
                old.Active = true;
				
				_context.SaveChanges();
            }
		}
		
		// POST api/Blog/Deactivate
		[HttpPost]
		[Authorize]
		[Route("Deactivate")]
		public void Deactivate(BlogPostViewModel post)
		{
			if (_context.BlogPosts.Any(bp => bp.Id == post.Id))
			{
                var old = _context.BlogPosts.First(bp => bp.Id == post.Id);
                old.Active = false;
				
				_context.SaveChanges();
            }
		}

		// GET api/Blog
		[HttpGet]
		public BlogPostViewModel Get(int id)
		{
			var post = BlogPostViewModel.FromBlogPost(_context.BlogPosts.FirstOrDefault(bp => bp.Id == id));
            post.HtmlContent = _blog.MarkDownToHtml(post.Content);
			
            return post;
        }
		
		// GET api/Blog
		[HttpGet]
		[Route("GetHtml")]
		public BlogPostViewModel GetHtml(BlogPostViewModel model)
		{
            return new BlogPostViewModel
            {
				HtmlContent = _blog.MarkDownToHtml(model.Content)
            };
        }

		// PUT api/Blog
		[HttpPut]
		[Authorize]
		public bool Put(BlogPostViewModel post)
		{
            if (!_context.BlogPosts.Any(bp => bp.Title == post.Title.Trim()))
			{
				var userId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "UserId").Value);
				
                var @new = new BlogPost
                {
                    AuthorId = userId,
                    Active = post.Active,
                    DatePosted = DateTime.Now,
					DateUpdated = DateTime.Now,
					Title = post.Title.Trim(),
					TitleUrl = _blog.GenerateUrlFromTitle(post.Title),
					Content = post.Content
            	};
                _context.BlogPosts.Add(@new);
				
                _context.SaveChanges();
                return true;
            } else
			{
                return false;
            }
        }
		
		// DELETE api/Blog
		[HttpDelete]
		[Authorize]
		public void Delete(BlogPostViewModel post)
		{
			if (_context.BlogPosts.Any(bp => bp.Id == post.Id))
			{
                var toRemove = _context.BlogPosts.First(bp => bp.Id == post.Id);
                _context.BlogPosts.Remove(toRemove);

                _context.SaveChanges();
            }
        }
	}
}