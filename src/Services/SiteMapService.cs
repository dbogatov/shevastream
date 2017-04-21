using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Shevastream.Extensions;
using Shevastream.Models;

namespace Shevastream.Services
{
	public interface ISiteMapService
	{
		/// <summary>
		/// Generates SiteMap from the posts / products / static pages
		/// </summary>
		/// <returns>Site map model (list of items)</returns>
		SiteMap GetSiteMap();
	}

	public class SiteMapService : ISiteMapService
	{
		private readonly IDataContext _context;
		private IUrlHelper _urlHelper;
		private readonly string HOST = "shevastream.com";
		private readonly string SCHEME = "https";

		public SiteMapService(
			IDataContext context,
			IUrlHelperFactory urlHelperFactory,
			IActionContextAccessor actionContextAccessor
		)
		{
			_context = context;
			_urlHelper = urlHelperFactory.GetUrlHelper(actionContextAccessor.ActionContext);
		}

		public SiteMap GetSiteMap()
		{
			var items =
				_context
				.BlogPosts
				.Where(post => post.Active)
				.ToList()
				.Select(post => new SiteMapItem
				{
					Loc = new Uri(_urlHelper.RouteUrl("Blog", new { title = post.TitleUrl, id = post.Id }, SCHEME, HOST)),
					ChangeFreq = ChangeFrequency.Daily,
					LastMod = post.DateUpdated
				})
				.ToList();

			items.AddRange(
				_context
				.Products
				.ToList()
				.Select(prod => new SiteMapItem
				{
					Loc = new Uri(_urlHelper.Action("Product", "Store", new { id = prod.Id }, SCHEME, HOST)),
					ChangeFreq = ChangeFrequency.Weekly
				})
				.ToList()
			);

			items.AddRange(
				new List<SiteMapItem> {
					new SiteMapItem
					{
						Loc = new Uri(_urlHelper.Action("Index", "Store", null, SCHEME, HOST)),
						ChangeFreq = ChangeFrequency.Monthly
					},
					new SiteMapItem
					{
						Loc = new Uri(_urlHelper.Action("Profile", "Store", null, SCHEME, HOST)),
						ChangeFreq = ChangeFrequency.Monthly
					},
					new SiteMapItem
					{
						Loc = new Uri(_urlHelper.Action("Contact", "Store", null, SCHEME, HOST)),
						ChangeFreq = ChangeFrequency.Monthly
					}
				}
			);

			return new SiteMap { Items = items };
		}
	}
}
