using Hadco.Services;
using Hadco.Services.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.OData;
using Hadco.Web.Models;
using Hadco.Web.Security;
using System.Text;
using Microsoft.Ajax.Utilities;

namespace Hadco.Web.Controllers
{
	/// <summary>
	/// 
	/// </summary>
    public class OccurrencesController : DefaultController<OccurrenceDto>
    {
        private readonly IOccurrenceService _occurrenceService;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="occurrenceService"></param>
        public OccurrencesController(IOccurrenceService occurrenceService) : base(occurrenceService)
		{
            _occurrenceService = occurrenceService;
        }
    }
}
