using System;
using System.ComponentModel.DataAnnotations;

namespace Shevastream.Models.Entities
{
	public class Feedback
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public string Email { get; set; }

		[Required]
		public string Subject { get; set; }

		[Required]
		public string Body { get; set; }
		
		[Required]
		public string Name { get; set; }

		public DateTime DateCreated { get; set; } = DateTime.Now;

		public override string ToString()
		{
			return $@"
				Feedback:
				
					Name: {Name}
					Email: {Email}
					Subject: {Subject}
					Body:
						{Body}";
		}
	}
}
