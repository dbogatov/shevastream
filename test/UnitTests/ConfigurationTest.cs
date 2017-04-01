using Microsoft.Extensions.Configuration;
using Xunit;

namespace Shevastream.Tests.UnitTests
{
	/// <summary>
	/// Unit tests which check that configuration has all required properties set.
	/// </summary>
	public class ConfigurationTest
	{
		private readonly IConfiguration _config;

		public ConfigurationTest()
		{
			// Generating configuration key-value store from file.
			_config =
				new ConfigurationBuilder()
				.AddJsonFile("appsettings.json", optional: false)
				.Build();
		}

		/// <summary>
		/// Helper method that asserts that the value is defined for the key.
		/// </summary>
		/// <param name="key">The key for which to check the value.</param>
		private void TestSpecificKey(string key)
		{
			Assert.NotNull(_config[key]);
		}
	}
}
