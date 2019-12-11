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
    ///     A controller for performing operations against Category data in a REST API
    /// </summary>
    public class CategoriesController : GenericController<CategoryDto>
    {
        private readonly ICategoryService _categoryService;

        /// <summary>
        /// 
        /// </summary>        
        /// <param name="categoryService"></param>
        public CategoriesController(ICategoryService categoryService)
            : base(categoryService)
        {

            _categoryService = categoryService;
        }


        /// <summary>
        ///     DEPRECATED: Use /HadcoShop
        ///     Returns the categories available for 15HADCO.SHOP selections.
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(PaginatedResult<CategoryDto>))]
        [ActionAuthorize("read")]
        [Route("api/Categories/15HadcoShop")]
        public HttpResponseMessage Get15HadcoShop()
        {
            return GetHadcoShop();
        }

        /// <summary>
        ///     DEPRECATED: Use /HadcoShopOverhead. 
        ///     Returns the categories available for 15HADCO.SHOP selections.
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(CategoryDto))]
        [ActionAuthorize("read")]
        [Route("api/Categories/15HadcoShopOverhead")]
        public HttpResponseMessage Get15HadcoShopOverhead()
        {
            return GetHadcoShopOverhead();
        }

        /// <summary>
        ///     Returns the categories available for 15HADCO.SHOP selections.
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(PaginatedResult<CategoryDto>))]
        [ActionAuthorize("read")]
        [Route("api/Categories/HadcoShop")]
        public HttpResponseMessage GetHadcoShop()
        {
            var result = _categoryService.GetHadcoShop();
            int resultCount = result.Count();
            return PaginatedResult<CategoryDto>.GetPaginatedResult(Request, result, resultCount, resultCount);
        }

        /// <summary>
        ///     Returns the categories available for 15HADCO.SHOP selections.
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(CategoryDto))]
        [ActionAuthorize("read")]
        [Route("api/Categories/HadcoShopOverhead")]
        public HttpResponseMessage GetHadcoShopOverhead()
        {
            return Request.CreateResponse(HttpStatusCode.OK, _categoryService.GetHadcoShopOverhead());
        }
    }
}
