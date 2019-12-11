using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http.OData;
using Hadco.Services;
using Hadco.Services.DataTransferObjects;
using Hadco.Web.Security;
using System.Net;
using System.Web.Http;

namespace Hadco.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class SettingsController : GenericController<SettingDto>
    {
        ISettingService _settingService;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="settingService"></param>
        public SettingsController(ISettingService settingService)
            : base(settingService)
        {
            _settingService = settingService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [ActionAuthorize("write")]
        public HttpResponseMessage Post(PostSettingDto dto)
        {
            return Request.CreateResponse(HttpStatusCode.OK, _settingService.Insert(dto));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [ActionAuthorize("read")]
        [Route("api/Settings/BreadCrumbIntervalInSeconds")]
        public HttpResponseMessage Get()
        {
            return Request.CreateResponse(HttpStatusCode.OK, _settingService.GetBreadCrumbIntervalInSeconds());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Route("api/Settings/BingApiKey")]
        public HttpResponseMessage GetBingApiKey()
        {
            var apiKey = ConfigurationManager.AppSettings["BingApiKey"];
            return Request.CreateResponse(HttpStatusCode.OK, new { apiKey });
        }
    }
}