using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Routing;
using Hadco.Common;

namespace Hadco.Web
{
    /// <summary>
    ///     Place for route configurations and other API configuration settings. This gets called once from Startup.cs
    /// </summary>
    public static class WebApiConfig
    {
        /// <summary>
        ///     Settings for the Web API get set in this method, including traditional routing configurations, JSON serializer settings, etc...
        /// </summary>
        /// <param name="config">The current configuration instance for the API.</param>
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Removing the XML formatter
            var appXmlType = config.Formatters.XmlFormatter.SupportedMediaTypes.FirstOrDefault(t => t.MediaType == "application/xml");
            config.Formatters.XmlFormatter.SupportedMediaTypes.Remove(appXmlType);

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // Capture exceptions for Application Insights:
            config.Filters.Add(new AiExceptionFilterAttribute());

            var jsonFormatter = config.Formatters.JsonFormatter;
            JsonSerializerSettings settings = jsonFormatter.SerializerSettings;

            settings.Converters.Add(new DayConverter());

            IsoDateTimeConverter dateConverter = new IsoDateTimeConverter
            {
                DateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.fffzzz",
                DateTimeStyles = System.Globalization.DateTimeStyles.AdjustToUniversal
            };
            settings.Converters.Add(dateConverter);

            // Only supporting the JSON formatter.
            jsonFormatter.SerializerSettings = settings;
            jsonFormatter.SerializerSettings.ContractResolver = new DeltaContractResolver(jsonFormatter);
            config.Formatters.Clear();
            config.Formatters.Add(jsonFormatter);

            config.Filters.Add(new HadcoExceptionFilterAttribute());
        }
    }
}
