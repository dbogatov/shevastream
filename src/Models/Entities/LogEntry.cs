using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EShop.Models.Enitites
{

	[Table("LogEntry")]
	public class LogEntry
	{
		[Key]
		public int Id { get; set; }
		public int Type { get; set; }
		public int TargetId { get; set; }
		public long Timestamp { get; set; }
	}
}