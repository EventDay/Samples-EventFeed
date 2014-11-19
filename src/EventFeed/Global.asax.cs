using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using EventDay.Api.Client.Configuration;
using EventDay.Api.Client.Mvc.Autofac;
using EventFeed.Controllers;

namespace EventFeed
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            IEventDayApiConfiguration configuration = ApiConfiguration.FromConfig();

            EventDayConfiguration.ForMvc<HomeController>()
                .Configure(configuration);
        }
    }
}