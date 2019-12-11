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
using Hadco.Common.DataTransferObjects;

namespace Hadco.Web.Controllers
{
    /// <summary>
    ///     A controller for performing operations against JobEquipment data in a REST API
    /// </summary>
    public class EmployeeJobEquipmentTimersController : GenericController<EmployeeJobEquipmentTimerDto>
    {
        private readonly IEmployeeJobEquipmentTimerService _employeeJobEquipmentTimerService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="employeeJobEquipmentTimerService"></param>
        public EmployeeJobEquipmentTimersController(IEmployeeJobEquipmentTimerService employeeJobEquipmentTimerService)
            : base(employeeJobEquipmentTimerService)
        {
            _employeeJobEquipmentTimerService = employeeJobEquipmentTimerService;
        }
                

        /// <summary>
        ///     Creates a new employee job equipment timer in the system.
        ///     The object must contain an EmployeeJobTimerID or an EmployeeTimerID
        ///     and a JobTimerID. If the EmployeeJobTimerID is null, the system will
        ///     try to find the EmployeeJobTimerID based on the EmployeeTimerID and
        ///     the JobTimerID. If an EmployeeJobTimer is not found, one will be created
        ///     with 0 labor minutes.
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Succesfully created the EmployeeJobEquipmentTimer.</response>
        /// <response code="401">Unauthorized access.</response>
        [ResponseType(typeof(EmployeeJobEquipmentTimerDto))]
        [ActionAuthorize("write")]
        public HttpResponseMessage Post(EmployeeJobEquipmentTimerDto dto)
        {
            return this.Post<EmployeeJobEquipmentTimerDto>(dto);
        }

        /// <summary>
        ///    Modifies an existing employee job equipment timer in the system.
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Successfully updated the employee timer.</response>
        /// <response code="401">Unauthorized access.</response>
        [ResponseType(typeof(EmployeeJobEquipmentTimerDto))]
        [ActionAuthorize("write")]
        public HttpResponseMessage Patch(int id, Delta<EmployeeJobEquipmentTimerDto> dto)
        {
            return this.Patch<EmployeeJobEquipmentTimerDto>(id, dto);
        }

        /// <summary>
        ///    Deletes an existing employee job equipment timer in the system.
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Successfully deleted the employee timer.</response>
        /// <response code="401">Unauthorized access.</response>
        [ResponseType(typeof(EmployeeJobEquipmentTimerDto))]
        [ActionAuthorize("delete")]
        public HttpResponseMessage Delete(int id)
        {
            return this.Delete<EmployeeJobEquipmentTimerDto>(id);
        }
    }
}
