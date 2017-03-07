using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Shevastream.Extensions
{
    public static class StringExtensions
    {
		public static bool Contains(this string source, string toCheck, StringComparison comp)
		{
			return source.IndexOf(toCheck, comp) >= 0;
		}

        public static string UnicodeSafeSubstring(this string str, int startIndex, int length)
        {
            if (str == null)
            {
                throw new ArgumentNullException("str");
            }

            if (startIndex < 0 || startIndex > str.Length)
            {
                throw new ArgumentOutOfRangeException("startIndex");
            }

            if (length < 0)
            {
                throw new ArgumentOutOfRangeException("length");
            }

            if (startIndex + length > str.Length)
            {
                throw new ArgumentOutOfRangeException("length");
            }

            if (length == 0)
            {
                return string.Empty;
            }

            var sb = new StringBuilder(length);

            int end = startIndex + length;

            var enumerator = StringInfo.GetTextElementEnumerator(str, startIndex);

            while (enumerator.MoveNext())
            {
                string grapheme = enumerator.GetTextElement();
                startIndex += grapheme.Length;

                if (startIndex > length)
                {
                    break;
                }

                // Skip initial Low Surrogates/Combining Marks
                if (sb.Length == 0)
                {
                    if (char.IsLowSurrogate(grapheme[0]))
                    {
                        continue;
                    }

                    UnicodeCategory cat = CharUnicodeInfo.GetUnicodeCategory(grapheme, 0);

                    if (cat == UnicodeCategory.NonSpacingMark || cat == UnicodeCategory.SpacingCombiningMark || cat == UnicodeCategory.EnclosingMark)
                    {
                        continue;
                    }
                }

                sb.Append(grapheme);

                if (startIndex == length)
                {
                    break;
                }
            }

            return sb.ToString();
        }

        public static string Truncate(this string value, int maxChars)
        {
            return new StringInfo(value).LengthInTextElements <= maxChars ? value : value.UnicodeSafeSubstring(0, maxChars) + "...";
        }

        public static string StripHTML(this string input)
        {
            return Regex.Replace(input, "<.*?>", String.Empty);
        }
    }

	/// <summary>
	/// ASCII transliterations of Unicode text
	/// </summary>
	public static partial class Unidecoder
	{
		/// <summary>
		/// Transliterate Unicode string to ASCII string.
		/// </summary>
		/// <param name="input">String you want to transliterate into ASCII</param>
		/// <param name="tempStringBuilderCapacity">
		///     If you know the length of the result,
		///     pass the value for StringBuilder capacity.
		///     InputString.Length*2 is used by default.
		/// </param>
		/// <returns>
		///     ASCII string. There are [?] (3 characters) in places of some unknown(?) unicode characters.
		///     It is this way in Python code as well.
		/// </returns>
		public static string Unidecode(this string input, int? tempStringBuilderCapacity = null)
		{
			if (string.IsNullOrEmpty(input))
			{
				return "";
			}

			if (input.All(x => x < 0x80))
			{
				return input;
			}


			// Unidecode result often can be at least two times longer than input string.
			var sb = new StringBuilder(tempStringBuilderCapacity ?? input.Length * 2);
			foreach (char c in input)
			{
				// Copypaste is bad, but sb.Append(c.Unidecode()); would be a bit slower.
				if (c < 0x80)
				{
					sb.Append(c);
				}
				else
				{
					int high = c >> 8;
					int low = c & 0xff;
					string[] transliterations;
					if (characters.TryGetValue(high, out transliterations))
					{
						sb.Append(transliterations[low]);
					}
				}
			}

			return sb.ToString();
		}

		/// <summary>
		/// Transliterate Unicode character to ASCII string.
		/// </summary>
		/// <param name="c">Character you want to transliterate into ASCII</param>
		/// <returns>
		///     ASCII string. Unknown(?) unicode characters will return [?] (3 characters).
		///     It is this way in Python code as well.
		/// </returns>
		public static string Unidecode(this char c)
		{
			string result;
			if (c < 0x80)
			{
				result = new string(c, 1);
			}
			else
			{
				int high = c >> 8;
				int low = c & 0xff;
				string[] transliterations;
				if (characters.TryGetValue(high, out transliterations))
				{
					result = transliterations[low];
				}
				else
				{
					result = "";
				}
			}

			return result;
		}
	}
}