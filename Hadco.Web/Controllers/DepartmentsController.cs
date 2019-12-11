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
    ///     A controller for performing operations against Department data in a REST API
    /// </summary>
    public class DepartmentsController : GenericController<DepartmentDto>
    {
        private readonly IDepartmentService _departmentService;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="departmentService"></param>
        public DepartmentsController(IDepartmentService departmentService)
            : base(departmentService)
        {
            _departmentService = departmentService;
        }

        /// <summary>
        ///     Gets the Departments in the system. By default it only returns currently active departments.
        /// </summary>
        /// <returns></returns>
        [ActionAuthorize("read")]
        [EndpointQueryStrings]
        public HttpResponseMessage Get()
        {
            if (FilterItems == null || FilterItems.Count == 0 && Pagination == null)
            {
                var result = _departmentService.GetFromCache().ToList();
                return PaginatedResult<DepartmentDto>.GetPaginatedResult(Request, result, result.Count, result.Count);
            }
            return this.Get<DepartmentDto>();
        }

        /// <summary>
        ///     Gets the Department in the system with the given department ID.
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(DepartmentDto))]
        [ActionAuthorize("read")]
        [Route("api/Departments/{id}")]
        public HttpResponseMessage Get(int id)
        {
            return this.Get<DepartmentDto>(id);
        }


        /// <summary>
        /// Patch the TrackedBy field for the department
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        [ActionAuthorize("write")]
        [Route("api/Departments/{id}")]
        public HttpResponseMessage Patch(int id, Delta<PatchDepartmentDto> dto)
        {
            if (dto == null)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, new Exception("A body is required for PATCH"));

            var item = _departmentService.Find(id);

            if (item == null)
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, new Exception("Item does not exist."));

            dto.Patch(item);
            Validate(item);
            if (!ModelState.IsValid)
                return CreateErrorResponse(ModelState);

            try
            {
                var result = _departmentService.Update(item);
                return Request.CreateResponse(result);
            }
            catch (ArgumentException e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, e);
            }
        }
        
    }
}
