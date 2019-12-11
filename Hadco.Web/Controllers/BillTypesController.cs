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

namespace Hadco.Web.Controllers
{
	/// <summary>
	/// 
	/// </summary>
    public class BillTypesController : GenericController<BillTypeDto>
    {
        private readonly IBillTypeService _billTypeService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="billTypeService"></param>
        public BillTypesController(IBillTypeService billTypeService) : base(billTypeService)
		{
            _billTypeService = billTypeService;
        }

        /// <summary>
        ///     Gets the BillTypes in the system for use in Trucking timers.
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(PaginatedResult<BillTypeDto>))]
        [ActionAuthorize("read")]
        public HttpResponseMessage Get()
        {
            var result = _billTypeService.GetFromCache().ToList();
            return PaginatedResult<BillTypeDto>.GetPaginatedResult(Request, result, result.Count, result.Count);
        }

    }
}
