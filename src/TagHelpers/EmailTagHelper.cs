using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Shevastream.TagHelpers
{
    [HtmlTargetElement("email", Attributes = "to")]
    public class EmailTagHelper : TagHelper
    {
        public string To { get; set; }
        public string Subject { get; set; } = "";

		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
			output.TagName = "a";
			output.TagMode = TagMode.StartTagAndEndTag;

			output.Attributes.Clear();
			output.Attributes.Add("href", $"mailto:{To}?subject={Subject.Replace(" ", "%20")}");
			output.Attributes.Add("target", "_top");

			output.Content.SetContent(To);
		}
    }
}