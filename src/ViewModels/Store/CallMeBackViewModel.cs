namespace EShop.ViewModels.Store
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