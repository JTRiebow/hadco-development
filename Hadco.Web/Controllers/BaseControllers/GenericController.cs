using Hadco.Services;
using Hadco.Services.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace Hadco.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="D"></typeparam>
    public class GenericController<D> : AuthorizedController
        where D : class, IDataTransferObject, new()
    {
        /// <summary>
        /// 
        /// </summary>
        protected IGenericService<D> _baseService;

        /// <summary>
        /// 
        /// </summary>
        public IGenericService<D> BaseService
        {
            get
            {
                return _baseService;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseService"></param>
        public GenericController(IGenericService<D> baseService)
        {
            _baseService = baseService;
        }
    }
}