using System.Web.Mvc;
using System.Web.Routing;

namespace Hadco.Web
{
    /// <summary>
    /// 
    /// </summary>
    public class RouteConfig
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="routes"></param>
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "WebApp",
                url: "{*url}",
                defaults: new { controller = "WebApp", action = "Index" }
            );
        }
    }
}