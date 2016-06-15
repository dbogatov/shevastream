using System;
using System.Linq;
using EShop.Models;

namespace EShop.Services
{
    public interface ISiteMapService
    {
        SiteMap GetSiteMap();
    }

    public class SiteMapService : ISiteMapService
    {
        private readonly DataContext _context;

        private readonly string URL = "https://shevastream.com";

        public SiteMapService(DataContext context)
        {
            _context = context;
        }

        public SiteMap GetSiteMap()
        {
            var items = 
                _context
                .BlogPosts.
                ToList().
                Select(post => new SiteMapItem
                {
                    Loc = new Uri($"{URL}/blog/{post.TitleUrl}"),
                    ChangeFreq = ChangeFrequency.Daily,
                    LastMod = post.DateUpdated
                }).ToList();

            items.Add(new SiteMapItem
            {
                Loc = new Uri(URL),
                ChangeFreq = ChangeFrequency.Hourly
            });

            return new SiteMap { Items = items };
        }
    }
}
