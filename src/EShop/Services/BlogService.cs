using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using EShop.Models.Enitites;
using EShop.ViewModels.Blog;
using Microsoft.EntityFrameworkCore;
using NickBuhro.Translit;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace EShop.Services
{
	public interface IBlogService
	{
		string MarkDownToHtml(string content);
		string GenerateUrlFromTitle(string title);
		IEnumerable<BlogPostViewModel> GetAllPosts();
		Task<BlogPostViewModel> GetPostByTitleAsync(string title, bool active = true);
		Task<BlogPostViewModel> GetPostByIdAsync(int id, bool active = true);
		Task<BlogPost> UpdatePostAsync(BlogPostViewModel post);
		void TogglePublish(BlogPostViewModel post, bool publish);
		Task<BlogPost> CreatePostAsync(BlogPostViewModel post);
		void RemovePost(BlogPostViewModel post);
		Task AddViewAsync(BlogPostViewModel post);

	}

	public class BlogService : IBlogService
	{
		private readonly DataContext _context;
		private readonly HttpContext _http;

		public BlogService(DataContext context, IHttpContextAccessor http)
		{
			_http = http.HttpContext;
			_context = context;
		}

		public string GenerateUrlFromTitle(string title)
		{
			title = Transliteration.CyrillicToLatin(title, Language.Ukrainian);

			// make it all lower case
			title = title.ToLower();

			// remove entities
			title = Regex.Replace(title, @"&\w+;", "");

			// remove anything that is not letters, numbers, dash, or space
			title = Regex.Replace(title, @"[^a-z0-9\-\s]", "");

			// replace spaces
			title = title.Replace(' ', '-');

			// collapse dashes
			title = Regex.Replace(title, @"-{2,}", "-");

			// trim excessive dashes at the beginning
			title = title.TrimStart(new[] { '-' });

			// if it's too long, clip it
			if (title.Length > 80)
			{
				title = title.Substring(0, 79);
			}

			// remove trailing dashes
			title = title.TrimEnd(new[] { '-' });

			return title;
		}

		public string MarkDownToHtml(string content)
		{
			return CommonMark.CommonMarkConverter.Convert(content);
		}

		public IEnumerable<BlogPostViewModel> GetAllPosts()
		{
			return _context
				.BlogPosts
				.Include(bp => bp.Author)
				.OrderByDescending(bp => bp.DatePosted)
				.Select(bp => new BlogPostViewModel
				{
					Id = bp.Id,
					AuthorName = bp.Author.NickName,
					DatePosted = bp.DatePosted,
					Title = bp.Title,
					TitleUrl = bp.TitleUrl,
					Active = bp.Active,
					Views = bp.Views,
					Preview = bp.Preview
				});
		}

		public async Task<BlogPostViewModel> GetPostByTitleAsync(string title, bool active = true)
		{
			if (_context.BlogPosts.Any(bp => (active ? bp.Active : true) && bp.TitleUrl == title))
			{
				var post =
					BlogPostViewModel.FromBlogPost(await _context
					.BlogPosts.Include(bp => bp.Author)
					.FirstAsync(
						bp => bp.TitleUrl == title
					));

				post.HtmlContent = MarkDownToHtml(post.Content); ;

				return post;
			}
			else
			{
				return null;
			}
		}

		public async Task<BlogPostViewModel> GetPostByIdAsync(int id, bool active = true)
		{
			if (_context.BlogPosts.Any(bp => (active ? bp.Active : true) && bp.Id == id))
			{
				var post =
					BlogPostViewModel.FromBlogPost(await _context
					.BlogPosts.Include(bp => bp.Author)
					.FirstAsync(
						bp => bp.Id == id
					));

				post.HtmlContent = MarkDownToHtml(post.Content); ;

				return post;
			}
			else
			{
				return null;
			}
		}

		public async Task<BlogPost> UpdatePostAsync(BlogPostViewModel post)
		{
			if (_context.BlogPosts.Any(bp => bp.Id == post.Id))
			{
				var userId = Convert.ToInt32(_http.User.Claims.FirstOrDefault(c => c.Type == "UserId").Value);

				var old = _context.BlogPosts.First(bp => bp.Id == post.Id);
				old.Title = post.Title.Trim();
				old.TitleUrl = GenerateUrlFromTitle(post.Title);
				old.Content = post.Content;
				old.Preview = MarkDownToHtml(post.Content.Substring(0, post.Content.IndexOf('\n')));
				old.DateUpdated = DateTime.Now;
				old.AuthorId = userId;
				old.Active = post.Active;

				await _context.SaveChangesAsync();

				return old;
			}

			return null;
		}

		public void TogglePublish(BlogPostViewModel post, bool publish)
		{
			if (_context.BlogPosts.Any(bp => bp.Id == post.Id))
			{
				var old = _context.BlogPosts.First(bp => bp.Id == post.Id);
				old.Active = publish;

				_context.SaveChanges();
			}
		}

		public async Task<BlogPost> CreatePostAsync(BlogPostViewModel post)
		{
			var userId = Convert.ToInt32(_http.User.Claims.FirstOrDefault(c => c.Type == "UserId").Value);

			var @new = new BlogPost
			{
				AuthorId = userId,
				Active = post.Active,
				DatePosted = DateTime.Now,
				DateUpdated = DateTime.Now,
				Title = post.Title.Trim(),
				TitleUrl = GenerateUrlFromTitle(post.Title),
				Content = post.Content
			};
			_context.BlogPosts.Add(@new);

			await _context.SaveChangesAsync();

			return @new;
		}

		public void RemovePost(BlogPostViewModel post)
		{
			if (_context.BlogPosts.Any(bp => bp.Id == post.Id))
			{
				var toRemove = _context.BlogPosts.First(bp => bp.Id == post.Id);
				_context.BlogPosts.Remove(toRemove);

				_context.SaveChanges();
			}
		}

		public async Task AddViewAsync(BlogPostViewModel post)
		{
			if (_context.BlogPosts.Any(bp => bp.Id == post.Id))
			{
				var old = _context.BlogPosts.First(bp => bp.Id == post.Id);
				old.Views++;

				await _context.SaveChangesAsync();
			}
		}
	}
}

