using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Hadco.Common;
using System.Net;
using Hadco.Services.DataTransferObjects;
using Hadco.Services;
using Hadco.Web.Security;

namespace Hadco.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class EmployeeTypesController : GenericController<EmployeeTypeDto>
    {

        private readonly IEmployeeTypeService _employeeTypeService;

        /// <summary>
        ///     The constructor for the EmployeeTypes Controller
        /// </summary>
        /// <param name="employeeTypeService"></param>
        public EmployeeTypesController(IEmployeeTypeService employeeTypeService)
            : base(employeeTypeService)
        {
            _employeeTypeService = employeeTypeService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [ActionAuthorize("read")]
        public HttpResponseMessage Get()
        {
            return this.Get<EmployeeTypeDto>();
        }
    }
}