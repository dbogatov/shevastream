using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Shevastream.Models.Entities;
using Shevastream.ViewModels.Blog;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Shevastream.Models;
using Microsoft.Extensions.Logging;
using Shevastream.Extensions;

namespace Shevastream.Services
{
	public interface IBlogService
	{
		/// <summary>
		/// Generate HTML from Markdown
		/// </summary>
		/// <param name="content">Markdown input</param>
		/// <returns>HTML output</returns>
		string MarkDownToHtml(string content);

		/// <summary>
		/// Generate TitleUrl property of the blog post
		/// </summary>
		/// <param name="title">Original post title</param>
		/// <returns>TitleUrl property of the post</returns>
		string GenerateUrlFromTitle(string title);

		/// <summary>
		/// Generates HTML version of the post preview
		/// </summary>
		/// <param name="content">Markdown post content</param>
		/// <returns>HTML preview</returns>
		string GeneratePreview(string content);

		/// <summary>
		/// Returns all available posts, including not active ones
		/// </summary>
		/// <returns>All available posts</returns>
		Task<IEnumerable<BlogPostViewModel>> GetAllPostsAsync();

		/// <summary>
		/// Returns the post given by the unique ID
		/// </summary>
		/// <param name="id">ID of the post to find</param>
		/// <param name="active">If true, and the post is not active, return null</param>
		/// <returns>Post given by ID or null, if post not found</returns>
		Task<BlogPostViewModel> GetPostByIdAsync(int id, bool active = true);

		/// <summary>
		/// Tries to find the post in the database, updates it and saves changes.
		/// If cannot find, returns null.
		/// </summary>
		/// <param name="post">Post to update</param>
		/// <returns>Updated post if post was found, null otherwise</returns>
		Task<BlogPost> UpdatePostAsync(BlogPostViewModel post);

		/// <summary>
		/// Creates the post in the database and return it.
		/// </summary>
		/// <param name="post">Post to create.</param>
		/// <returns>Created post.</returns>
		Task<BlogPost> CreatePostAsync(BlogPostViewModel post);

		/// <summary>
		/// Increments a view counter for the given post.
		/// Saves changes in the database.
		/// </summary>
		/// <param name="post">Post for which to increase view counter.</param>
		/// <returns>New number of views, or null if post does not exist in the database.</returns>
		Task<int?> AddViewAsync(BlogPostViewModel post);

		/// <summary>
		/// Return postsNum of latest posts.
		/// </summary>
		/// <param name="postsNum">Number of post to return.</param>
		/// <returns>Returns postsNum (or fewer) latest posts in a chronological order.</returns>
		Task<IEnumerable<BlogPostViewModel>> GetLatestPostsAsync(int postsNum);
	}

	public class BlogService : IBlogService
	{
		private readonly IDataContext _context;
		private readonly IAuthService _auth;
		private readonly ITransliterationService _translit;
		private readonly ILogger<BlogService> _logger;

		public BlogService(
			IDataContext context,
			IAuthService auth,
			ITransliterationService translit,
			ILogger<BlogService> logger
		)
		{
			_auth = auth;
			_context = context;
			_translit = translit;
			_logger = logger;
		}

		public string GenerateUrlFromTitle(string title)
		{
			title = _translit.CyrillicToLatin(title);

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

		public async Task<IEnumerable<BlogPostViewModel>> GetAllPostsAsync()
		{
			return await _context
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
				})
				.ToListAsync();
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

				post.HtmlContent = MarkDownToHtml(post.Content);

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
				var userId = _auth.GetCurrentUserId();
				var user = await _context.Users.FindAsync(userId);

				var old = _context.BlogPosts.First(bp => bp.Id == post.Id);
				old.Title = post.Title.Trim();
				old.TitleUrl = GenerateUrlFromTitle(post.Title);
				old.Content = post.Content;
				old.Preview = GeneratePreview(post.Content);
				old.DateUpdated = DateTime.Now;
				old.Author = user;
				old.Active = post.Active;

				await _context.SaveChangesAsync();

				_logger.LogInformation(LoggingEvents.Blog.AsInt(), $"Post {old.Id} has been updated");

				return old;
			}

			return null;
		}

		public async Task<BlogPost> CreatePostAsync(BlogPostViewModel post)
		{
			var userId = _auth.GetCurrentUserId();
			var user = await _context.Users.FindAsync(userId);

			var @new = new BlogPost
			{
				Author = user,
				Active = post.Active,
				DatePosted = DateTime.Now,
				DateUpdated = DateTime.Now,
				Title = post.Title.Trim(),
				TitleUrl = GenerateUrlFromTitle(post.Title),
				Preview = GeneratePreview(post.Content),
				Content = post.Content
			};
			_context.BlogPosts.Add(@new);

			await _context.SaveChangesAsync();

			_logger.LogInformation(LoggingEvents.Blog.AsInt(), $"Post {@new.Id} has been created");

			return @new;
		}

		public async Task<int?> AddViewAsync(BlogPostViewModel post)
		{
			if (_context.BlogPosts.Any(bp => bp.Id == post.Id))
			{
				var old = _context.BlogPosts.First(bp => bp.Id == post.Id);
				old.Views++;

				await _context.SaveChangesAsync();

				return old.Views;
			}
			else
			{
				return null;
			}
		}

		public async Task<IEnumerable<BlogPostViewModel>> GetLatestPostsAsync(int postsNum = 3)
		{
			return (await _context
				.BlogPosts
				.Where(bp => bp.Active)
				.OrderByDescending(bp => bp.DatePosted)
				.Take(postsNum)
				.ToListAsync())
				.Select(bp => BlogPostViewModel.FromBlogPost(bp));
		}

		public string GeneratePreview(string content)
		{
			return MarkDownToHtml(
				content.TrimStart().IndexOf('\n') > 0 ?
				content.Substring(0, content.TrimStart().IndexOf('\n')) :
				content
			);
		}
	}
}

