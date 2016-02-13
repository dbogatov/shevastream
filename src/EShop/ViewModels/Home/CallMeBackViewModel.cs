using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EShop.ViewModels.Home
{
	public class CallMeBackViewModel
	{
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