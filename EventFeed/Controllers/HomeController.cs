using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;
using EventDay.Api.Client;

namespace EventFeed.Controllers
{
    public class HomeController : Controller
    {
        private readonly IEventDayApiClient _apiClient;

        public HomeController(IEventDayApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        // GET: Home
        public async Task<ActionResult> Index()
        {
            EventSummaryState[] states = await _apiClient.GetEventSummariesAsync();
            
            var feed = new SyndicationFeed("My Company", "All Upcoming Events", null);
            feed.Authors.Add(new SyndicationPerson("events@mycompany.com", "Events", "http://events.mycompany.com"));

            var items = new List<SyndicationItem>();

            foreach (var @event in states)
            {
                var item = new SyndicationItem(@event.Title, @event.Description, @event.ShortUrl)
                {
                    PublishDate = DateTime.UtcNow
                };

                item.ElementExtensions.Add(new XElement("startDate", @event.StartDateUtc.ToString("O")).CreateReader());
                item.ElementExtensions.Add(new XElement("endDate", @event.EndDateUtc.ToString("O")).CreateReader());
                item.ElementExtensions.Add(new XElement("city", @event.City).CreateReader());
                item.ElementExtensions.Add(new XElement("state", @event.State).CreateReader());
                
                items.Add(item);
            }

            feed.Items = items;

            Encoding encoding = Encoding.UTF8;

            using(var writer = new StringWriter())
            using (var xml = new XmlTextWriter(writer))
            {
                feed.SaveAsRss20(xml);
                string rss = writer.ToString();
                return Content(rss, "application/rss+xml", encoding);
            }

        }
    }
}