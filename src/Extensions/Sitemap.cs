using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc;

namespace Shevastream.Extensions
{
	public class SiteMapResult : IActionResult
	{
		private SiteMap _siteMap;

		public SiteMapResult(SiteMap siteMap)
		{
			_siteMap = siteMap;
		}

		public Task ExecuteResultAsync(ActionContext context)
		{
			context.HttpContext.Response.ContentType = "text/xml";

			using (var output = XmlWriter.Create(context.HttpContext.Response.Body))
			{
				WriteTo(output);
			}

			return Task.FromResult(0);
		}

		public void WriteTo(XmlWriter writer)
		{
			XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";
			var xml = new XDocument(
				new XDeclaration("1.0", "utf-8", "yes"),
				new XElement(ns + "urlset",
						_siteMap.Items
						.Select(
							item => new XElement(
								ns + "url",
								new XElement(ns + "loc", item.Loc),
								item.LastMod.HasValue ? new XElement(ns + "lastmod", item.LastMod.Value.ToString("s") + "Z") : null,
								item.ChangeFreq != ChangeFrequency.NotSet ? new XElement(ns + "changefreq", item.ChangeFreq.ToString().ToLower()) : null,
								item.Priority.HasValue ? new XElement(ns + "priority", item.Priority.Value) : null
							)
						)
				)
			);
			xml.Save(writer);
		}
	}

	public enum ChangeFrequency
	{
		NotSet, Always, Hourly, Daily, Weekly, Monthly, Yearly, Never
	}

	public class SiteMap
	{
		public List<SiteMapItem> Items { get; set; }
	}

	public class SiteMapItem
	{
		public Uri Loc { get; set; }
		public DateTime? LastMod { get; set; }
		public ChangeFrequency ChangeFreq { get; set; }
		public double? Priority { get; set; }
	}
}