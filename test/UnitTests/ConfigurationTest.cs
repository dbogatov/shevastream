using System;
using Microsoft.Extensions.Configuration;
using Shevastream.Extensions;
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
				.AddJsonFile("appsettings.testing.json", optional: true)
				.Build();
		}

		[Theory]
		[InlineData("Data:ConnectionString")]
		[InlineData("Data:PrivacyPolicy:Title")]
		[InlineData("Data:PrivacyPolicy:Content")]
		[InlineData("Data:Products:0:Name")]
		[InlineData("Data:Products:0:Description")]
		[InlineData("Data:Products:0:Information")]
		[InlineData("Data:Products:0:VideoData:Url")]
		[InlineData("Data:Users:0:FullName")]
		[InlineData("Data:Users:0:NickName")]
		[InlineData("Data:Users:0:ImageUrl")]
		[InlineData("Data:Users:0:Password")]
		[InlineData("Data:Users:0:Position")]
		[InlineData("Data:Users:0:Occupation")]
		[InlineData("Logging:MinLogLevel")]
		public void StringSettingsExist(string key)
		{
			Assert.NotNull(_config[key]);
		}

		[Theory]
		[InlineData("Data:PrivacyPolicy:Id")]
		[InlineData("Data:PrivacyPolicy:AuthorId")]
		[InlineData("Data:Products:0:Id")]
		[InlineData("Data:Products:0:Cost")]
		[InlineData("Data:Users:0:Id")]
		public void IntegerSettingsExist(string key)
		{
			Assert.NotNull(_config[key]);
			Assert.True(int.TryParse(_config[key], out _));
		}

		[Theory]
		[InlineData("Data:Products:0:VideoData:HasVideo")]
		public void BooleanSettingsExist(string key)
		{
			Assert.NotNull(_config[key]);
			Assert.True(bool.TryParse(_config[key], out _));
		}

		[Theory]
		[InlineData("Data:PrivacyPolicy:DatePosted")]
		public void DateSettingsExist(string key)
		{
			Assert.NotNull(_config[key]);
			Assert.True(DateTime.TryParse(_config[key], out _));
		}

		[Theory]
		[InlineData("Data:Products:0:ImageUrls")]
		[InlineData("Data:Products:0:Characteristics")]
		public void ArrayOfStringsSettingsExist(string key)
		{
			var strings = _config.StringsFromArray(key);

			Assert.NotEmpty(strings);
		}
	}
}
