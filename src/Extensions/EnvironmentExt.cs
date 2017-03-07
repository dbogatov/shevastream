using Microsoft.AspNetCore.Hosting;

namespace Shevastream.Extensions
{
	public static class EnvironmentExtensions
	{
		/// <summary>
		/// Convenient shortcut to check if EnvironmentName is "Testing"
		/// </summary>
		/// <param name="env">Environment object to check</param>
		/// <returns>True if EnvironmentName is "Testing", false otherwise</returns>
		public static bool IsTesting(this IHostingEnvironment env)
		{
			return env.EnvironmentName == "Testing";
		}
	}
}