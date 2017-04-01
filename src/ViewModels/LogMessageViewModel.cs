using Microsoft.Extensions.Logging;

namespace Shevastream.ViewModels
{
	public class LogMessageViewModel
	{
		public LogLevel Severity { get; set; }
		public string Source { get; set; }
		public string AuxiliaryData { get; set; } = "";
		public string Message { get; set; }
		public int Category { get; set; } = 0;
	}
}
