using System.Text.RegularExpressions;
using NickBuhro.Translit;

namespace EShop.Services
{
	public interface IBlogService
	{
		string MarkDownToHtml(string content);
		string GenerateUrlFromTitle(string title);
	}

	public class BlogService : IBlogService
	{
		public string GenerateUrlFromTitle(string title)
		{
			title = Transliteration.CyrillicToLatin(title, Language.Ukrainian);

			// make it all lower case
			title = title.ToLower();

			// remove entities
			title = Regex.Replace(title, @"&\w+;", "");

			// remove anything that is not letters, numbers, dash, or space
			title = Regex.Replace(title, @"[^a-z0-9\-\s]", "");

			// replace spaces
			title = title.Replace(' ', '-');

			// collapse dashes
			title = Regex.Replace(title, @"-{2,}", "-");

			// trim excessive dashes at the beginning
			title = title.TrimStart(new[] { '-' });

			// if it's too long, clip it
			if (title.Length > 80)
			{
				title = title.Substring(0, 79);
			}

			// remove trailing dashes
			title = title.TrimEnd(new[] { '-' });

			return title;
		}

		public string MarkDownToHtml(string content)
		{
			return CommonMark.CommonMarkConverter.Convert(content);
		}
	}
}

