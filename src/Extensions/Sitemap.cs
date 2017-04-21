using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc;

namespace Shevastream.Extensions
{
	/// <summary>
	/// Special action result that returns sitemap
	/// </summary>
	public class SiteMapResult : IActionResult
	{
		public SiteMap SiteMap { get; private set; }

		public SiteMapResult(SiteMap siteMap)
		{
			SiteMap = siteMap;
		}

		public Task ExecuteResultAsync(ActionContext context)
		{
			context.HttpContext.Response.ContentType = "text/xml";

			using (var output = XmlWriter.Create(context.HttpContext.Response.Body))
			{
				WriteTo(output);
			}

			return Task.CompletedTask;
		}

		/// <summary>
		/// Generate XML from the SiteMap list
		/// </summary>
		/// <param name="writer">XmlWriter to generate to</param>
		public void WriteTo(XmlWriter writer)
		{
			XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";
			var xml = new XDocument(
				new XDeclaration("1.0", "utf-8", "yes"),
				new XElement(ns + "urlset",
						SiteMap.Items
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

	/// <summary>
	/// Available SiteMap frequencies
	/// </summary>
	public enum ChangeFrequency
	{
		NotSet, Always, Hourly, Daily, Weekly, Monthly, Yearly, Never
	}
	
	/// <summary>
	/// List of SiteMap items
	/// </summary>
	public class SiteMap
	{
		public List<SiteMapItem> Items { get; set; }
	}

	/// <summary>
	/// A single SiteMap item
	/// </summary>
	public class SiteMapItem
	{
		public Uri Loc { get; set; }
		public DateTime? LastMod { get; set; }
		public ChangeFrequency ChangeFreq { get; set; }
		public double? Priority { get; set; }
	}
}
