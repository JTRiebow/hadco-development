using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using Hadco.Common.Enums;
using Hadco.Services;

namespace Hadco.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class CsvController : AuthorizedController
    {
        readonly ICsvService _csvService;
        public CsvController(ICsvService csvService)
        {
            _csvService = csvService;
        }

        /// <summary>
        /// Gets the difference between total time clocked in and time allocated load timers and downtime as a csv
        /// </summary>
        /// <param name="csvType"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="departmentID"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/CSV/{csvType}")]
        public HttpResponseMessage GetCsv(CsvType csvType,DateTime startDate, DateTime endDate, [FromUri]DepartmentName[] departmentID)
        {
                var csv = _csvService.GetCsv(csvType, startDate, endDate, departmentID);
                return ReturnCsv(startDate, endDate, csv);
        }

        private HttpResponseMessage ReturnCsv(DateTime startDate, DateTime endDate, Csv csv)
        {
            if (csv?.Data == null)
            {
                return Request.CreateResponse(HttpStatusCode.NoContent);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(csv.Data, Encoding.UTF8, "text/csv");
            response.Content.Headers.ContentDisposition =
                new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                {
                    FileName = csv.FileName,
                    Name = csv.FileName
                };
            return response;
        }
    }
}