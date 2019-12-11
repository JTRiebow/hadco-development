using System;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.OData;
using Hadco.Common;
using Hadco.Services;
using Hadco.Services.DataTransferObjects;
using Hadco.Web.Security;

namespace Hadco.Web.Controllers
{
    /// <summary>
    /// Allows access to the LoadTimerEntry entities.
    /// </summary>
    public class LoadTimerEntriesController : GenericController<LoadTimerEntryDto>
    {
        ILoadTimerEntryService _loadTimerEntryservice;

        /// <summary>
        /// Constructor for the LoadTimerEntrysController.
        /// </summary>
        /// <param name="loadTimerEntryservice"></param>
        public LoadTimerEntriesController(ILoadTimerEntryService loadTimerEntryservice)
            : base(loadTimerEntryservice)
        {
            _loadTimerEntryservice = loadTimerEntryservice;
        }

        #region Base Endpoints

        /// <summary>
        /// Gets the loadTimerEntry with the given ID.
        /// </summary>
        /// <param name="id">The LoadTimerEntry ID.</param>
        /// <returns></returns>
        [ActionAuthorize("read")]
        [ResponseType(typeof(LoadTimerEntryDto))]
        public HttpResponseMessage Get(int id)
        {
            return this.Get<LoadTimerEntryDto>(id);
        }

        /// <summary>
        /// Inserts a new loadTimerEntry into the database.
        /// </summary>
        /// <param name="dto">The loadTimerEntry to be entered into the database.</param>
        /// <param name="startNow">If true, override start time with current server time</param>
        /// <param name="stopNow">If true, override stop time with current server time</param>
        /// <returns></returns>
        [ActionAuthorize("write")]
        [ResponseType(typeof(LoadTimerEntryDto))]
        public HttpResponseMessage Post(LoadTimerEntryDto dto, bool startNow = false, bool stopNow = false)
        {
            if (startNow)
            {
                dto.StartTime = DateTimeOffset.Now.RoundToMinute();
            }
            if (stopNow)
            {
                dto.EndTime = DateTimeOffset.Now.RoundToMinute();
            }
            return this.Post<LoadTimerEntryDto>(dto);
        }

        /// <summary>
        /// Updates a loadTimerEntry already stored in the database.
        /// </summary>
        /// <param name="id">The LoadTimerEntry ID.</param>
        /// <param name="dto">All of the data needing to be updated on the loadTimerEntry.</param>
        /// <param name="startNow">If true, override start time with current server time</param>
        /// <param name="stopNow">If true, override stop time with current server time</param>
        /// <returns></returns>
        [ActionAuthorize("write")]
        [ResponseType(typeof(LoadTimerEntryDto))]
        public HttpResponseMessage Patch(int id, Delta<LoadTimerEntryDto> dto, bool startNow = false, bool stopNow = false)
        {
            if (startNow)
            {
                dto.TrySetPropertyValue("StartTime", DateTimeOffset.Now.RoundToMinute());
            }
            if (stopNow)
            {
                dto.TrySetPropertyValue("EndTime", DateTimeOffset.Now.RoundToMinute());
            }
            return this.Patch<LoadTimerEntryDto>(id, dto);
        }

        /// <summary>
        /// Deletes the loadTimerEntry in the database with the given ID, if it exists.
        /// </summary>
        /// <param name="id">The ID of the loadTimerEntry to be deleted.</param>
        /// <returns></returns>
        [ActionAuthorize("delete")]
        public HttpResponseMessage Delete(int id)
        {
            return this.Delete<LoadTimerEntryDto>(id);
        }

        #endregion Base Endpoints
    }
}