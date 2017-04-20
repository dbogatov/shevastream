using System.ComponentModel.DataAnnotations;

namespace Shevastream.ViewModels.Store
{
	public class CallMeBackViewModel
	{
		[Required]
		public string Phone { get; set; }


		public override string ToString()
		{

			return $@"
				Call Request:
				
					Phone: {Phone}
				";
		}
	}
}
