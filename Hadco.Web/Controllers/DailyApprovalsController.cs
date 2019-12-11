using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using Hadco.Common;
using Hadco.Common.DataTransferObjects;
using Hadco.Services;
using Hadco.Services.DataTransferObjects;
using System.Web.Http.OData;

namespace Hadco.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class DailyApprovalsController : AuthorizedController
    {
        private IDailyApprovalService _dailyApprovalService;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dailyApprovalService"></param>
        public DailyApprovalsController(IDailyApprovalService dailyApprovalService)
        {
            _dailyApprovalService = dailyApprovalService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="employeeID"></param>
        /// <param name="day"></param>
        /// <param name="departmentID"></param>
        /// <returns></returns>
       [Route("api/Employee/{employeeID}/Day/{day}/Department/{departmentID}/DailyApproval")]
        public HttpResponseMessage Get(int employeeID, DateTime day, int departmentID)
        {
            var dto = _dailyApprovalService.Get(employeeID, day, departmentID);
            if (dto == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
            return Request.CreateResponse(dto);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public HttpResponseMessage Patch(int id, Delta<DailyApprovalPatchDto> dto)
        {
            var result = _dailyApprovalService.Update(id, dto);
            return Request.CreateResponse(result);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="departmentID"></param>
        /// <param name="dto"></param>
        /// <param name="employeeID"></param>
        /// <param name="day"></param>
        /// <returns></returns>
        [HttpPatch]
        [ResponseType(typeof(DailyApprovalDto))]
        [Route("api/Employee/{employeeID}/Day/{day}/Department/{departmentID}/DailyApproval")]
        public HttpResponseMessage Patch(int employeeID, DateTime day, int departmentID, Delta<DailyApprovalPatchDto> dto)
        {
            try
            {
                var result = _dailyApprovalService.Update(employeeID, day, departmentID, dto);
                return Request.CreateResponse(result);
            } catch (Exception ex)
            {
                var result = Request.CreateResponse(HttpStatusCode.BadRequest, dto);
                result.ReasonPhrase = $"Unauthorized Approval Requested.  {ex.Message}";
                return result;
            }
        }
    }
}