using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Hadco.Web.Security
{
	/// <summary>
	/// 
	/// </summary>
    public class CustomOAuthBearerProvider : OAuthBearerAuthenticationProvider
    {
        private const string NAME = "access_token";

		/// <summary>
		/// 
		/// </summary>
        public CustomOAuthBearerProvider()
        {
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
        public override Task RequestToken(OAuthRequestTokenContext context)
        {
            var value = context.Request.Query.Get(NAME);

            if (!string.IsNullOrEmpty(value))
            {
                context.Token = value;
                return Task.FromResult<object>(null);
            }
            
            return base.RequestToken(context);
        }
    }
}