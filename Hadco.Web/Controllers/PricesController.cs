using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.OData;
using Hadco.Services;
using Hadco.Services.DataTransferObjects;
using Hadco.Web.Models;
using Hadco.Web.Security;

namespace Hadco.Web.Controllers
{
    /// <summary>
    ///     A controller for performing operations against Price data in a REST API
    /// </summary>
    public class PricesController : GenericController<PriceDto>
    {
        private readonly IPriceService _priceService;
        private readonly IPricingService _pricingService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="priceService"></param>
        /// <param name="pricingService"></param>
        public PricesController(IPriceService priceService, IPricingService pricingService)
            : base(priceService)
        {
            _priceService = priceService;
            _pricingService = pricingService;
        }

        /// <summary>
        /// Returns current Prices objects for a given pricing id
        /// </summary>
        /// <param name="pricingID">Integer Pricing ID.</param>
        /// <returns></returns>
        /// <response code="404">Not found.</response>
        /// <response code="200">OK</response>
        [HttpGet]
        [Route("api/Prices/Pricing/{pricingID}")]
        [ActionAuthorize("read")]
        public HttpResponseMessage GetCurrentPricesForID(int pricingID)
        {
            int resultCount;
            int totalResultCount;
            var prices = (IEnumerable<dynamic>)_priceService.GetPricesByID(pricingID, out resultCount, out totalResultCount);
            if (prices.Any())
            {
                return PaginatedResult<dynamic>.GetPaginatedResult(Request, prices, resultCount, totalResultCount);
            }
            return Request.CreateErrorResponse(HttpStatusCode.NotFound, new Exception("Price data not found"));
        }

        /// <summary>
        /// Get Price History
        /// </summary>
        /// <param name="customerTypeID">Integer Customer Type ID (Development = 1, Residential = 2, Outside = 3, Metro = 4).</param>
        /// <param name="billTypeID">Integer Bill Type ID  (Hourly = 1, Ton = 2, Load = 3)</param>
        /// <param name="jobID">Integer Job ID (Should be null unless CustomerTypeID = 1, Development) </param>
        /// <param name="customerID">Integer Customer ID (Should by null unless customerTypeID = 2, Residential)</param>
        /// <param name="phaseID">Integer Phase ID (Should be null unless customerTypeID = 3 or 4, Outside or Metro) </param>
        /// <returns></returns>
        /// <response code="404">Not found.</response>
        /// <response code="200">OK</response>
        [HttpGet]
        [Route("api/Prices/PriceHistory")]
        [ActionAuthorize("read")]
        public HttpResponseMessage GetPriceHistory(int customerTypeID, int billTypeID, int? jobID = null, int? customerID = null, int? phaseID = null)
        {          
            var priceHistory = (IEnumerable<dynamic>)_pricingService.GetPriceHistory(customerTypeID, billTypeID, jobID, customerID, phaseID);
            return Request.CreateResponse(HttpStatusCode.OK, priceHistory);
        }

        /// <summary>
        /// Updates pricing for a given price ID
        /// </summary>
        /// <param name="priceID">Integer Pricing ID.</param>
        /// <param name="dto"></param>
        /// <returns></returns>
        /// <response code="404">Not found.</response>
        /// <response code="200">OK</response>
        [HttpPatch]
        [Route("api/Prices/{priceID}")]
        [ActionAuthorize("write")]
        public HttpResponseMessage UpdatePrice(int priceID, Delta<PriceDto> dto)
        {
            return this.Patch(priceID, dto);
        }

        /// <summary>
        /// Adds new Price object
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        /// <response code="404">Not found.</response>
        /// <response code="200">OK</response>
        [HttpPost]
        [Route("api/Prices/")]
        [ActionAuthorize("write")]
        public HttpResponseMessage AddPrice(PriceDto dto)
        {
            return this.Post(dto);
        }

    }
}