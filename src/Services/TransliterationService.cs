using System;
using EShop.Extensions;

namespace EShop.Services
{
	public interface ITransliterationService
	{
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
