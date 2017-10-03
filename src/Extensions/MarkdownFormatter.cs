using System;
using CommonMark;
using CommonMark.Syntax;

namespace Shevastream.Extensions
{
	/// <summary>
	/// Class ti override CommonMark default behavior.
	/// </summary>
	class MyFormatter : CommonMark.Formatters.HtmlFormatter
	{
		public MyFormatter(System.IO.TextWriter target, CommonMarkSettings settings)
			: base(target, settings)
		{
		}

		protected override void WriteInline(Inline inline, bool isOpening, bool isClosing, out bool ignoreChildNodes)
		{
			if (
				// Do not output "title" and "description"
				isClosing
				&& inline.Tag == InlineTag.Image
			)
			{
				ignoreChildNodes = true;
				return;
			}

			if (
				// Start and end of each node may be visited separately
				isOpening
				// Verify that the inline element is one that should be modified
				&& inline.Tag == InlineTag.Image
				// Verify that the formatter should output HTML and not plain text
				&& !this.RenderPlainTextInlines.Peek())
			{
				// Instruct the formatter NOT to process all nested nodes automatically
				ignoreChildNodes = true;

				this.Write($"<img id=\"{Math.Abs(inline.TargetUrl.GetHashCode())}\" class=\"img-responsive img-rounded\" src=\"");
				this.WriteEncodedUrl(inline.TargetUrl);
				this.Write($"\" alt=\"{inline.LiteralContent}\" />");
				this.Write($"<label for=\"{Math.Abs(inline.TargetUrl.GetHashCode())}\">{inline.FirstChild.LiteralContent}</label>");
			}
			else
			{
				// In all other cases the default implementation will output the correct HTML
				base.WriteInline(inline, isOpening, isClosing, out ignoreChildNodes);
			}
		}
	}
}
