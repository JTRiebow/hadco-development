using Hadco.Services;
using Hadco.Services.DataTransferObjects;
using Hadco.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.OData;
using Hadco.Web.Security;
using Hadco.Common;

namespace Hadco.Web.Controllers
{
    /// <summary>
    ///     A controller for performing operations against Job data in a REST API
    /// </summary>
    public class JobsController : GenericController<JobDto>
    {
        private readonly IJobService _jobService;
        private readonly IPhaseService _phaseService;
        private readonly ICategoryService _categoryService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jobService"></param>
        /// <param name="phaseService"></param>
        /// <param name="categoryService"></param>
        public JobsController(IJobService jobService, IPhaseService phaseService, ICategoryService categoryService)
            : base(jobService)
        {
            _jobService = jobService;
            _phaseService = phaseService;
            _categoryService = categoryService;
        }

        /// <summary>
        ///     Gets the Jobs in the system. By default it only returns currently active jobs.
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(PaginatedResult<JobDto>))]
        [ActionAuthorize("read")]
        [EndpointQueryStrings]
        public HttpResponseMessage Get(DateTimeOffset? lastUpdated = null)
        {
            if (((FilterItems.AllKeys.Contains("filter") && !FilterItems["filter"].Contains("Status")) || !FilterItems.AllKeys.Contains("filter")) && !lastUpdated.HasValue)
            {
                FilterItems.Add("Status", "1");
            }
            int resultCount, totalResultCount;
            var result = _jobService.GetAllWithDateFilter(lastUpdated, FilterItems, Pagination, out resultCount, out totalResultCount);
            return PaginatedResult<JobDto>.GetPaginatedResult(Request, result, resultCount, totalResultCount);
            //return this.Get<JobDto>();
        }

        /// <summary>
        ///     Gets the Phases in the system.
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(PaginatedResult<PhaseDto>))]
        [ActionAuthorize("read")]
        [EndpointQueryStrings]
        [Route("api/Jobs/{jobID}/Phases")]
        public HttpResponseMessage Get(int jobID)
        {
            if ((FilterItems.AllKeys.Contains("filter") && !FilterItems["filter"].Contains("Status")) || !FilterItems.AllKeys.Contains("filter"))
            {
                FilterItems.Add("Status", "1");
            }
            FilterItems.Add("JobID", jobID.ToString());
            int resultCount;
            int totalResultCount;
            var result = _phaseService.GetAll(FilterItems, Pagination, out resultCount, out totalResultCount);         
            return PaginatedResult<PhaseDto>.GetPaginatedResult(Request, result, resultCount, totalResultCount);            
        }

        /// <summary>
        ///     Gets the Categories in the system.
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(PaginatedResult<CategoryDto>))]
        [ActionAuthorize("read")]
        [EndpointQueryStrings]
        [Route("api/Jobs/{jobID}/Phases/{phaseID}/Categories")]
        public HttpResponseMessage Get(int jobID, int phaseID)
        {
            if ((FilterItems.AllKeys.Contains("filter") && !FilterItems["filter"].Contains("Status")) || !FilterItems.AllKeys.Contains("filter"))
            {
                FilterItems.Add("Status", "1");
            }
            FilterItems.Add("JobID", jobID.ToString());
            FilterItems.Add("PhaseID", phaseID.ToString());
            int resultCount;
            int totalResultCount;
            var result = _categoryService.GetAll(FilterItems, Pagination, out resultCount, out totalResultCount);
            return PaginatedResult<CategoryDto>.GetPaginatedResult(Request, result, resultCount, totalResultCount);            
        }
    }
}
