using Hadco.Services;
using Hadco.Services.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using System.Web.Http.OData;
using Hadco.Web.Models;
using Hadco.Web.Security;

namespace Hadco.Web.Controllers
{
	/// <summary>
	/// This controller already contains all of the default endpoints.
	/// </summary>
	/// <typeparam name="D"></typeparam>
    public abstract class DefaultController<D> : GenericController<D>
        where D : class, IDataTransferObject, new()
    {
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="baseService"></param>
        public DefaultController(IGenericService<D> baseService)
            : base(baseService)
        {
        }

		/// <summary>
		/// Get a list of objects based on provided filters and pagination values.
		/// </summary>
		/// <returns></returns>
        [ActionAuthorize("read")]
        public virtual HttpResponseMessage Get()
        {
            return this.Get<D>();
        }

		/// <summary>
		/// Get an object specified by the given id.
		/// </summary>
		/// <param name="id">id value for the object to retrieve</param>
		/// <returns></returns>
        [ActionAuthorize("read")]
        public virtual HttpResponseMessage Get(int id)
        {
            return this.Get<D>(id);
        }

        /// <summary>
        ///     Create a new instance of the object.
        /// </summary>
        /// <param name="dto">The data transfer object to be created.</param>
        /// <returns>The newly created object.</returns>
        [ActionAuthorize("write")]
        public virtual HttpResponseMessage Post(D dto)
        {
            return this.Post<D>(dto);
        }

        /// <summary>
        ///     Update the given entity
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        [ActionAuthorize("write")]
        public virtual HttpResponseMessage Put(int id, D dto)
        {
            return this.Put<D>(id, dto);
        }

        /// <summary>
        ///     Update the entity with the given id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        [ActionAuthorize("write")]
        public virtual HttpResponseMessage Patch(int id, Delta<D> dto)
        {
            return this.Patch<D>(id, dto);
        }

        /// <summary>
        ///     Delete the entity with the given id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ActionAuthorize("delete")]
        public virtual HttpResponseMessage Delete(int id)
        {
            return this.Delete<D>(id);
        }
    }
}
