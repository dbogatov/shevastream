using Microsoft.AspNetCore.Razor.TagHelpers;

namespace EShop.TagHelpers
{
    [HtmlTargetElement("thumbnail", Attributes = "img, title, price, product-id")]
    public class ThumbnailTagHelper : TagHelper
    {
		[HtmlAttributeName("img")]
        public string Image { get; set; }
        public string Title { get; set; }
		public string Price { get; set; }
		public int ProductId { get; set; }

		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
			output.TagName = "div";
			output.TagMode = TagMode.StartTagAndEndTag;

			output.Attributes.Clear();
			output.Attributes.Add("class", "col-md-6 col-sm-8 col-xs-12");

			output.Content.SetHtmlContent(
				$@"
					<div class='thumbnail'>
						<div class='caption-img' style='background: url({Image});'></div>
						<div class='caption-details'>
							<h3>{Title}</h3>
							<span class='price'>{Price} UAH</span>
						</div>
						<a href='/Store/Product/{ProductId}'><div class='caption-link'><i class='fa fa-plus'><!-- --></i></div></a>
					</div>
				"
			);
		}
    }
}