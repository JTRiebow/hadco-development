using Hadco.Common;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.ModelBinding;
using System.Web.Http.OData;

namespace Hadco.Web.Controllers
{
	/// <summary>
	/// 
	/// </summary>
    public abstract class BaseController : ApiController
    {
        /// <summary>
        /// 
        /// </summary>
        public const string FILE = "file";
        /// <summary>
        /// 
        /// </summary>
        public const string DISPLAY_ORDER = "displayorder";
        /// <summary>
        /// 
        /// </summary>
        public const string UPDATED_BY_ID = "UpdatedById";
        /// <summary>
        /// 
        /// </summary>
        public const string UPDATED_ON = "UpdatedOn";
        
        // <summary>
        // 
        // </summary>
        //public const string CREATED_ON = "CreatedOn";
        
        /// <summary>
        /// 
        /// </summary>
        public const string CREATED_BY_ID = "CreatedById";
        /// <summary>
        /// 
        /// </summary>
        public const string ORDER_BY = "orderby";
        /// <summary>
        /// 
        /// </summary>
        public const string SKIP = "skip";
        /// <summary>
        /// 
        /// </summary>
        public const string TAKE = "take";

		/// <summary>
		/// 
		/// </summary>
        public virtual string DefaultGetRouteName
        {
            get
            {
                return "DefaultApi";
            }
        }

        private NameValueCollection _filterItems;
		/// <summary>
		/// 
		/// </summary>
        public NameValueCollection FilterItems
        {
            get
            {
                return _filterItems = _filterItems ?? Request.RequestUri.ParseQueryString();
            }
        }

        private IEnumerable<OrderBy> _orderBy;
        private bool _orderBySet = false;

        /// <summary>
        ///     A property that returns the OrderBy information from the query string.
        /// </summary>
        protected IEnumerable<OrderBy> OrderBy
        {
            get
            {
                if (!_orderBySet && _orderBy == null)
                {
                    try
                    {
                        _orderBySet = true;
                        List<OrderBy> orderBy = null;
                        if (Request.GetQueryString(ORDER_BY) != null)
                        {
                            orderBy = new List<OrderBy>();
                            foreach (string orderItem in Request.GetQueryString(ORDER_BY).Split(','))
                            {
                                string[] items = orderItem.Split(' ');
                                if (items.Length == 2)
                                    orderBy.Add(new OrderBy { Direction = ((OrderDirection)Enum.Parse(typeof(OrderDirection), items[1], true)), Field = items[0] });
                                else
                                    orderBy.Add(new OrderBy { Direction = OrderDirection.Asc, Field = items[0] });
                            }
                        }
                        _orderBy = orderBy;
                    }
                    catch (Exception)
                    {
                        _orderBySet = true;
                        _orderBy = null;
                    }
                }
                return _orderBy;
            }
        }

        private Pagination _pagination;
        private bool _paginationSet = false;

        /// <summary>
        ///     Gets the pagination information from the query string
        /// </summary>
        public Pagination Pagination
        {
            get
            {
                if (!_paginationSet && _pagination == null)
                {
                    try
                    {
                        int skip = 0;
                        int take = int.MaxValue;
                        // Skip=10, Take=10, OrderBy=Name,DateCreated desc
                        int.TryParse(Request.GetQueryString(SKIP), out skip);
                        int.TryParse(Request.GetQueryString(TAKE), out take);

                        if (skip == 0 && take == 0 && OrderBy == null)
                            _pagination = null;
                        else
                            _pagination = new Pagination() { OrderBy = this.OrderBy, Take = (take == 0 ? int.MaxValue : take), Skip = skip };
                    }
                    catch (Exception)
                    {
                        _pagination = null;
                    }
                }
                return _pagination;
            }
        }

        /// <summary>
        ///     For CORs
        /// </summary>
        /// <returns>Returns an HTTP 200 always.</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        public HttpResponseMessage Options()
        {
            var response = new HttpResponseMessage();
            response.StatusCode = HttpStatusCode.OK;
            return response;
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="ModelState"></param>
		/// <returns></returns>
        protected HttpResponseMessage CreateErrorResponse(ModelStateDictionary ModelState)
        {
            string message = ModelState.Values.Where(x => x.Errors.Count != 0).First().Errors[0].ErrorMessage;
            string errorMessage;

            if (!string.IsNullOrEmpty(message))
                errorMessage = message;
            else if (ModelState.Values.Where(x => x.Errors.Count != 0).First().Errors[0].Exception != null)
                errorMessage = ModelState.Values.Where(x => x.Errors.Count != 0).First().Errors[0].Exception.Message;
            else
                errorMessage = "The model state is invalid.";

            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, new Exception(errorMessage));
        }

        /// <summary>
        ///     Returns the name of the controller
        /// </summary>
        public string ControllerName
        {
            get
            {
                return this.GetType().Name.Replace("Controller", string.Empty);
            }
        }
    }
}
