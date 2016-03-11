using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EShop.Services;

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