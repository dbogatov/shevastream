namespace Shevastream.ViewModels
{
	public class RegisterDiviceViewModel
	{
		public string DeviceToken { get; set; }

		public override string ToString()
		{

			return $@"
				Register Device:
				
					DeviceToken: {DeviceToken}
				";
		}
	}
}