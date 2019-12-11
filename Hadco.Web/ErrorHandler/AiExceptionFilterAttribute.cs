using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Filters;
using Microsoft.ApplicationInsights;

namespace Hadco.Web
{
    /// <summary>
    /// https://docs.microsoft.com/en-us/azure/application-insights/app-insights-asp-net-exceptions#exceptions
    /// </summary>
    public class AiExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext != null && actionExecutedContext.Exception != null)
            {
                var ai = new TelemetryClient();
                ai.TrackException(actionExecutedContext.Exception);
            }
            base.OnException(actionExecutedContext);
        }
    }
}