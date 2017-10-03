using Shevastream.Services;
using Xunit;

namespace Shevastream.Tests.UnitTests.Services
{
	public class TransliterationServiceTest
	{
		[Theory]
		[InlineData("Слово", "Slovo")]
		[InlineData("два слова", "dva slova")]
		[InlineData("два слова и цифра 12", "dva slova i tsifra 12")]
		[InlineData("сложные буквы й я ы е ь ю э ж х щ ш г", "slozhnye bukvy y ya y e ' yu e zh kh shch sh g")]
		[InlineData(
			"Жебрівський: у суботу електроенергія в Авдіївці не з'явиться", 
			"Zhebrivs'kiy: u subotu elektroenergiya v Avdiyivtsi ne z'yavit'sya"
		)]
		public void ConvertsCyrillicToLatin(string cyrillic, string latin)
		{
			// Arrange
			var transliterationService = new TransliterationService();
			
			// Act
			var result = transliterationService.CyrillicToLatin(cyrillic);

			// Assert
			Assert.Equal(latin, result);
		}
	}
}

