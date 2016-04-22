namespace EShop.ViewModels
{
	public class RegisterDiviceViewModel
	{
		public string DeviceToken { get; set; }
		public int UserId { get; set; }


		public override string ToString()
		{

			return $@"
				Register Device:
				
					DeviceToken: {DeviceToken}
					UserId: {UserId}
				";
		}
	}
}