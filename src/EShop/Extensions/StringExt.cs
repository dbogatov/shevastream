using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace EShop.Extensions
{
    public static class StringExtensions
    {
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
}