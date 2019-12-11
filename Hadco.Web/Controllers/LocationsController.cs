
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.OData;
using Hadco.Common.DataTransferObjects;
using Hadco.Services;
using Hadco.Services.DataTransferObjects;
using Hadco.Web.Security;
using Microsoft.Owin.Security;
using System.Collections.Generic;

namespace Hadco.Web.Controllers
{
    /// <summary>
    /// Allows access to the Locations entities.
    /// </summary>
    public class LocationsController : AuthorizedController
    {
        ILocationService _locationService;

        /// <summary>
        /// Constructor for the LocationsController.
        /// </summary>
        /// <param name="locationService"></param>
        public LocationsController(ILocationService locationService)
        {
            _locationService = locationService;
        }

        /// <summary>
        /// Gets the most recent GPS coordinates for the employee with the given employee id
        /// </summary>
        /// <param name="employeeID"></param>
        /// <returns></returns>
        [HttpGet]
        [ResponseType(typeof(GPSCoordinates))]
        [Route("api/Locations/MostRecent/{employeeID}")]
        public HttpResponseMessage MostRecentLocation(int employeeID)
        {
            var result = _locationService.GetMostRecentLocation(employeeID);
            if (result == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        /// <summary>
        /// Gets GPS coordinates for the employee with the given employee id 
        /// between the start and end times. Does not incule clock ins/outs.
        /// </summary>
        /// <param name="employeeID"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="departmentID">Optional</param>
        /// <returns></returns>
        [HttpGet]
        [ResponseType(typeof(IEnumerable<GPSCoordinates>))]
        [Route("api/Locations/Employee/{employeeID}")]
        public HttpResponseMessage Get(int employeeID, DateTimeOffset startTime, DateTimeOffset endTime, int? departmentID = null)
        {
            var result = _locationService.Get(employeeID, startTime, endTime, departmentID);
            if (result == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }


        /// <summary>
        /// Inserts a new Location into the database.
        /// </summary>
        /// <param name="dto">The location to be entered into the database.</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Locations")]
        [ResponseType(typeof(LocationPostResponseDto))]
        public HttpResponseMessage Post(LocationDto dto)
        {
            return  Request.CreateResponse(_locationService.Insert(dto));
        }

    }
}