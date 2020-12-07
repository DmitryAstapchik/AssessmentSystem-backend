using System.Web.Http;
using AssessmentSystem.Filters;

namespace AssessmentSystem
{
    /// <summary>
    /// Represents a configuraion for Web API.
    /// </summary>
    public static class WebApiConfig
    {
        /// <summary>
        /// Registers a Web API.
        /// </summary>
        /// <param name="config">A <see cref="HttpConfiguration"/>.</param>
        public static void Register(HttpConfiguration config)
        {
            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional });

#if DEBUG
            config.Filters.Add(new BasicAuthenticationFilter());
#endif
        }
    }
}