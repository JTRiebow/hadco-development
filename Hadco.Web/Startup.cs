using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.IO;
using Swashbuckle.Application;
using Hadco.Services;
using Hadco.Web.Security;
using Thinktecture.IdentityModel;

namespace Hadco.Web
{
	/// <summary>
	///
	/// </summary>
    public class Startup
    {
        private static HttpConfiguration _httpConfiguration;

		/// <summary>
		///
		/// </summary>
        public static HttpConfiguration HttpConfiguration
        {
            get
            {
                if (_httpConfiguration == null)
                {
                    _httpConfiguration = new HttpConfiguration();
                }
                return _httpConfiguration;
            }
        }

		/// <summary>
		///
		/// </summary>
        public static NinjectHttpResolver NinjectResolver;

		/// <summary>
		///
		/// </summary>
		/// <param name="app"></param>
        public void Configuration(IAppBuilder app)
        {
            var configuration = HttpConfiguration;
            NinjectHttpContainer.RegisterModules(configuration, NinjectHttpModules.Modules);
            NinjectResolver = new NinjectHttpResolver(NinjectHttpModules.Modules);
            WebApiConfig.Register(configuration);
            MappingProfile.Configure();
            SwaggerConfig.Register(configuration);

            // Read and pass any configuration values from the web.conf file and pass them here.
            //   Make sure to pass these values into the OAuthAuthorizationServerOptions object being passed to the auth
            //   server.
            // For example:
            //   bool allowInsecureHttp = bool.Parse(ConfigurationManager.AppSettings["allowInsecureHttp"]);

            app.UseOAuthAuthorizationServer(new OAuthAuthorizationServerOptions
            {
				// Example of passing in a custom configuration value to the server.
				AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromHours(3),
				Provider = new AuthorizationServerProvider()
            });

            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions() { Provider = new CustomOAuthBearerProvider() });

            ClaimsAuthorization.CustomAuthorizationManager = new AuthorizationManager();

            app.UseWebApi(configuration);
        }
    }
}