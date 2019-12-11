using Hadco.Services;
using Hadco.Services.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.OData;

namespace Hadco.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class NotesController : GenericController<NoteDto>
    {
        INoteService _noteService;
        IPermissionsService _permissionService;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="noteService"></param>
        /// <param name="permissionService"></param>
        public NotesController(INoteService noteService, IPermissionsService permissionService) : base(noteService)
        {
            _noteService = noteService;
            _permissionService = permissionService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage Get(int id)
        {
            return this.Get<NoteDto>(id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage Post(NoteDto dto)
        {
            return this.Post<NoteDto>(dto);
        }
        
        /// <summary>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPatch]
        public HttpResponseMessage Patch(int id, Delta<NoteDto> dto)
        {     
            return this.Patch<NoteDto>(id, dto);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Employee/{employeeID}/Day/{day}/DepartmentID/{departmentID}/Notes")]
        [ResponseType(typeof(IEnumerable<NoteDto>))]
        public HttpResponseMessage Get(int employeeID, DateTime day, int departmentID)
        {
            var notes = _noteService.GetNotes(employeeID, day, departmentID);
            return Request.CreateResponse(notes);
        }

        /// <summary>
        ///  Resolve a note with the given ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Notes/{id}/Resolve")]
        public HttpResponseMessage ResolveNote(int id)
        {
            var note = _noteService.ResolveNote(id);
            if (note == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
            return Request.CreateResponse(HttpStatusCode.OK, note);
        }
    }
}