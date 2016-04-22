using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EShop.Models.Enitites
{
	[Table("PushPair")]
	public class PushPair
	{
		public int UserId { get; set; }
		[Key]
		public string DeviceToken { get; set; }

		// navigation properties		
		[ForeignKey("UserId")]
		public User User { get; set; }
	}
}