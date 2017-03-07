using System;
using Shevastream.Extensions;

namespace Shevastream.Services
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
