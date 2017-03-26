using System;
using Xunit;

namespace Shevastream.Tests.UnitTests
{
	[Collection("The Collection")]
	/// <summary>
	/// Tests environment settings
	/// </summary>
	public class EnvironmentTest
	{
		[Fact]
		/// <summary>
		/// Checks that environment is set to testing.!--
		/// If not, other tests will fail.
		/// </summary>
		public void TestingEnvironmentSet()
		{
			var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"); ;

			Assert.Equal("Testing", env);
		}
	}
}