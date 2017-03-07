using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shevastream.Models.Enitites
{

	[Table("SSFeedback")]
	public class Feedback
	{
		[Key]
		public int Id { get; set; }
		public string Email { get; set; }
		public string Subject { get; set; }
		public string Body { get; set; }
		public string Name { get; set; }

		[Timestamp]
		public byte[] Timestamp { get; set; }

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