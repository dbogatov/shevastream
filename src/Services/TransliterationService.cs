using Shevastream.Extensions;

namespace Shevastream.Services
{
	public interface ITransliterationService
	{
		/// <summary>
		/// Translit text from cyrillic to latin.
		/// Leave unchanged if already latin.
		/// </summary>
		/// <param name="text">Text to translit</param>
		/// <returns>Translitted text</returns>
		string CyrillicToLatin(string text);
	}

	public class TransliterationService : ITransliterationService
	{
		public string CyrillicToLatin(string text)
		{
			return text.Unidecode();
		}
	}
}
