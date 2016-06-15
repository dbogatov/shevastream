using System;
using System.Collections.Generic;
using System.Linq;
using EShop.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;

namespace EShop.Services
{
    public interface ISiteMapService
    {
        SiteMap GetSiteMap();
    }

    public class SiteMapService : ISiteMapService
    {
        private readonly DataContext _context;
        private readonly HttpContext _http;
        private IUrlHelper _urlHelper;
        private readonly string HOST = "shevastream.com";
        private readonly string SCHEME = "https";

        public SiteMapService(
            DataContext context,
            IHttpContextAccessor http,
            IUrlHelperFactory urlHelperFactory,
            IActionContextAccessor actionContextAccessor
        )
        {
            _http = http.HttpContext;
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
                    Loc = new Uri(_urlHelper.RouteUrl("Blog", new { title = post.TitleUrl }, SCHEME, HOST)),
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
