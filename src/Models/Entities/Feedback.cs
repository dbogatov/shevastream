using System;
using System.ComponentModel.DataAnnotations;

namespace Shevastream.Models.Entities
{
    public class Feedback
	{
		[Key]
		public int Id { get; set; }
		public string Email { get; set; }
		public string Subject { get; set; }
		public string Body { get; set; }
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