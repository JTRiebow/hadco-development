using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;

namespace Hadco.Web.Models
{
    /// <summary>
    ///     This is a class that can be used when there is a collection of items of paginated items to be returned.
    /// </summary>
    public class PaginatedResult<D>
    {
        /// <summary>
        ///     The collection of items
        /// </summary>
        public IEnumerable<D> Items { get; set; }

        /// <summary>
        ///     The number of items returned
        /// </summary>
        public int ResultCount { get; set; }

        /// <summary>
        ///     The total number of items in the result set
        /// </summary>
        public int TotalResultCount { get; set; }
        
        /// <summary>
        ///     Creates an <see cref="HttpResponseMessage"/> with a <see cref="PaginatedResult&lt;D&gt;"/> as the contents of the body
        /// </summary>
        /// <param name="request">The <see cref="HttpRequestMessage"/> that is being responded to.</param>
        /// <param name="items">The IEnumerable items that are to be added to the response</param>
        /// <param name="resultCount">The total number of items that are actually being returned</param>
        /// <param name="totalResultCount">The total number of items that exists in the larger, non-paginated collection</param>
        /// <returns></returns>
        public static HttpResponseMessage GetPaginatedResult(HttpRequestMessage request, IEnumerable<D> items, int resultCount, int totalResultCount)
        {
            var paginatedResult = new PaginatedResult<D>();
            paginatedResult.Items = items;
            paginatedResult.ResultCount = resultCount;
            paginatedResult.TotalResultCount = totalResultCount;

            return request.CreateResponse(paginatedResult);
        }
    }
}