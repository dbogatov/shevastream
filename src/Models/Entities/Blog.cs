using System;
using System.ComponentModel.DataAnnotations;

namespace Shevastream.Models.Entities
{
	/// <summary>
	/// Class to represent a single blog post
	/// </summary>
	public class BlogPost
	{
		[Key]
		public int Id { get; set; }

		[Display(Name = "Publish")]
		/// <summary>
		/// True if post needs to be visible
		/// </summary>
		public bool Active { get; set; }
		public DateTime DatePosted { get; set; } = DateTime.Now;
		public DateTime DateUpdated { get; set; }

		// [AlternateKey] Fluent API
		public string Title { get; set; }
		// [AlternateKey] Fluent API
		/// <summary>
		/// A piece of url that resembles a title of the post and is unique
		/// </summary>
		public string TitleUrl { get; set; }

		public string Content { get; set; }

		/// <summary>
		/// HTML code that contains a preview part of the post
		/// </summary>
		public string Preview { get; set; }

		/// <summary>
		/// Number of times the post has been viewed (displayed)
		/// </summary>
		public int Views { get; set; } = 1;

		/// <summary>
		/// The user that last modified the post
		/// </summary>
		public User Author { get; set; }
	}
}
