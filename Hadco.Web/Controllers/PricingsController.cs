using Hadco.Services;
using Hadco.Services.DataTransferObjects;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.OData;
using Hadco.Common.Enums;
using Hadco.Web.Security;

namespace Hadco.Web.Controllers
{
    /// <summary>
    ///     A controller for performing operations against Pricing data in a REST API
    /// </summary>
    public class PricingsController : GenericController<PricingDto>
    {

        private IPricingService _pricingService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pricingService"></param>
        public PricingsController(IPricingService pricingService)
            : base(pricingService)
        {
            _pricingService = pricingService;
        }

        /// <summary>
        /// Get pricing Grid
        /// </summary>
        /// <param name="customerTypeID">Integer Customer Type ID (Development = 1, Residential = 2, Outside = 3, Metro = 4).</param>
        /// <param name="billTypeID">Integer Bill Type ID  (Hourly = 1, Ton = 2, Load = 3)</param>
        /// <returns></returns>
        /// <response code="404">Not found.</response>
        /// <response code="200">OK</response>
        [HttpGet]
        [Route("api/Pricings/PriceGrid/")]
        [ActionAuthorize("read")]
        public HttpResponseMessage GetPriceGrid(int customerTypeID, int billTypeID)
        {
            var grid = (IEnumerable<dynamic>)_pricingService.GetPriceGrid(customerTypeID, billTypeID);

            return Request.CreateResponse(HttpStatusCode.OK, grid);
        }

        /// <summary>
        /// Get Expanded Pricing information by pricing id
        /// </summary>
        /// <returns></returns>
        /// <response code="404">Not found.</response>
        /// <response code="200">OK</response>
        [HttpGet]
        [ResponseType(typeof(ExpandedPricingDto))]
        [Route("api/Pricings/{pricingID}")]
        [ActionAuthorize("read")]
        public HttpResponseMessage GetExpandedPricing(int pricingID)
        {
            var result = (ExpandedPricingDto)_pricingService.GetCurrentPricing(pricingID);

            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        /// <summary>
        /// Patch new pricing
        /// </summary>
        /// <returns></returns>
        /// <response code="404">Not found.</response>
        /// <response code="200">OK</response>
        [ResponseType(typeof(PricingDto))]
        [Route("api/Pricings/{pricingID}")]
        [ActionAuthorize("write")]
        public HttpResponseMessage Patch(int pricingID, Delta<PricingDto> pricing)
        {
            return this.Patch<PricingDto>(pricingID, pricing);
        }

        /// <summary>
        /// Post new pricing
        /// </summary>
        /// <returns></returns>
        /// <response code="404">Not found.</response>
        /// <response code="200">OK</response>
        [ResponseType(typeof(PricingDto))]
        [Route("api/Pricings/")]
        [ActionAuthorize("write")]
        public HttpResponseMessage Post(PricingDto dto)
        {
            if (!dto.CanOverlap && _pricingService.IsOverlapping(dto))
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Overlapping Pricings");
            }

            return Request.CreateResponse(HttpStatusCode.OK, _pricingService.Insert(dto));
        }

        /// <summary>
        /// Gets a list of all Jobs, Customer or Runs(Phases) with the Customer Type ID and Bill Type ID.
        /// </summary>
        /// <returns></returns>
        /// <response code="404">Not found.</response>
        /// <response code="200">OK</response>
        [HttpGet]
        [Route("api/PricingList")]
        [ActionAuthorize("read")]
        public HttpResponseMessage GetCustomerTypePricing(CustomerTypeName customerTypeID, BillTypeName billTypeID)
        {
            var result = (IEnumerable<dynamic>)_pricingService.GetCustomerJobOrPhaseList((int)customerTypeID, (int)billTypeID);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }
    }
}

