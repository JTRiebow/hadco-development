using Hadco.Services;
using Hadco.Services.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using Hadco.Web.Models;

namespace Hadco.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class NoteTypesController : GenericController<NoteTypeDto>
    {
        private readonly INoteTypeService _noteTypeService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="noteTypeService"></param>
        public NoteTypesController(INoteTypeService noteTypeService) : base(noteTypeService)
        {
            _noteTypeService = noteTypeService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public HttpResponseMessage Get()
        {
            var result = _noteTypeService.GetFromCache().ToList();
            return PaginatedResult<NoteTypeDto>.GetPaginatedResult(Request, result, result.Count, result.Count);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public HttpResponseMessage Get(int id)
        {
            return this.Get<NoteTypeDto>(id);
        }
    }
}