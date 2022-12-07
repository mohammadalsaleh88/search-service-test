using System.Net.Http.Formatting;
using System.Web.Http;

namespace SearchService
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "{controller}/{action}/{id}",
                defaults: new { controller = "ServiceSearch", action = "GetAll", id = RouteParameter.Optional }
            );

            config.Formatters.Clear();

            config.Formatters.Add(new JsonMediaTypeFormatter());
        }
    }
}