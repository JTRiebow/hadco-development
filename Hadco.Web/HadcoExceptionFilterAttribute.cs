using Hadco.Common;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;

namespace Hadco.Web
{
    /// <summary>
    /// 
    /// </summary>
    public class HadcoExceptionFilterAttribute : ExceptionFilterAttribute
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="context"></param>
        public override void OnException(HttpActionExecutedContext context)
        {
            if (context.Exception is UnauthorizedDataAccessException)
            {
                context.Response = context.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, context.Exception.Message);
            }
            else
            {                
                context.Response = context.Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetExceptionMessage(context.Exception));
            }
        }

        private string GetExceptionMessage(Exception exception, string message = null)
        {
            if (message == null)
                message = exception.Message;
            else
                message = message + Environment.NewLine + exception.Message;

            if(exception.InnerException != null)
            {
                message = GetExceptionMessage(exception.InnerException, message);
            }
            return message;
        }
    }
}