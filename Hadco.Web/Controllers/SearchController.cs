using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.OData;
using Hadco.Services;
using Hadco.Services.DataTransferObjects;
using Hadco.Web.Security;

namespace Hadco.Web.Controllers
{
    /// <summary>
    /// Allows access to the Search entities.
    /// </summary>
    public class SearchController : AuthorizedController
    {
        readonly ISearchService _searchservice;

        /// <summary>
        /// Constructor for the SearchsController.
        /// </summary>
        /// <param name="searchservice"></param>
        public SearchController(ISearchService searchservice)
        {
            _searchservice = searchservice;
        }


        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Employees/{employeeID}/Search/{date}")]
        [ResponseType(typeof(EmployeeSearchDto))]
        public HttpResponseMessage Get(int employeeID, DateTime date)
        {
            var result = _searchservice.Search(employeeID, date);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

    }
}