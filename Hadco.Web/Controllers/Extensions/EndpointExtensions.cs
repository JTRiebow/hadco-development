using Hadco.Services.DataTransferObjects;
using Hadco.Web.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using System.Web.Http.OData;
using Hadco.Common;

namespace Hadco.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public static class EndpointExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="D"></typeparam>
        /// <param name="controller"></param>
        /// <returns></returns>
        public static HttpResponseMessage Get<D>(this GenericController<D> controller)
            where D : class, IDataTransferObject, new()
        {
            int resultCount;
            int totalResultCount;
            var result = controller.BaseService.GetAll(controller.FilterItems, controller.Pagination, out resultCount, out totalResultCount);
            return PaginatedResult<D>.GetPaginatedResult(controller.Request, result, resultCount, totalResultCount);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="D"></typeparam>
        /// <param name="controller"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static HttpResponseMessage Get<D>(this GenericController<D> controller, int id)
            where D : class, IDataTransferObject, new()
        {
            var item = controller.BaseService.Find(id);
            return controller.Request.CreateResponse(item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="D"></typeparam>
        /// <param name="controller"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public static HttpResponseMessage Post<D>(this GenericController<D> controller, D dto)
            where D : class, IDataTransferObject, new()
        {
            if (!controller.ModelState.IsValid)
            {
                controller.CreateErrorResponse(controller.ModelState);
            }

            try
            {
                D result = controller.BaseService.Insert(dto);
                var uri = new Uri(controller.Url.Link(controller.DefaultGetRouteName, new { id = result.ID, controller = controller.ControllerName }));

                var response = controller.Request.CreateResponse(result);
                response.Headers.Location = uri;
                return response;
            }
            catch (ArgumentException e)
            {
                return controller.Request.CreateErrorResponse(HttpStatusCode.BadRequest, e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="D"></typeparam>
        /// <param name="controller"></param>
        /// <param name="id"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public static HttpResponseMessage Put<D>(this GenericController<D> controller, int id, D dto)
            where D : class, IDataTransferObject, new()
        {
            if (!controller.ModelState.IsValid)
                return controller.CreateErrorResponse(controller.ModelState);

            try
            {
                if (controller.BaseService.Exists(id))
                {
                    D result = controller.BaseService.Update(dto);
                    return controller.Request.CreateResponse(result);
                }
                else
                {
                    D result = controller.BaseService.Insert(dto);
                    var uri = new Uri(controller.Url.Link(controller.DefaultGetRouteName, new { id = result.ID, controller = controller.ControllerName }));

                    var response = controller.Request.CreateResponse(result);
                    response.Headers.Location = uri;
                    return response;
                }
            }
            catch (ArgumentException e)
            {
                return controller.Request.CreateErrorResponse(HttpStatusCode.BadRequest, e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="D"></typeparam>
        /// <param name="controller"></param>
        /// <param name="id"></param>
        /// <param name="dto"></param>
        /// <param name="ignoreProperties"></param>
        /// <returns></returns>
        public static HttpResponseMessage Patch<D>(this GenericController<D> controller, int id, Delta<D> dto, params string[] ignoreProperties)
            where D : class, IDataTransferObject, new()
        {
            if (dto == null)
                return controller.Request.CreateErrorResponse(HttpStatusCode.BadRequest, new Exception("A body is required for PATCH"));

            D item = controller.BaseService.Find(id, false);

            if (item == null)
                return controller.Request.CreateErrorResponse(HttpStatusCode.NotFound, new Exception("Item does not exist."));

            foreach (var ignoreProperty in ignoreProperties)
            {
                if (typeof(D).GetProperties().Any(x => x.Name == ignoreProperty))
                {
                    dto.TrySetPropertyValue(ignoreProperty, typeof(D).GetProperty(ignoreProperty)?.GetValue(item));
                }
            }

            dto.Patch(item);
            controller.Validate(item);
            if (!controller.ModelState.IsValid)
                return controller.CreateErrorResponse(controller.ModelState);

            try
            {
                D result = controller.BaseService.Update(item);
                return controller.Request.CreateResponse(result);
            }
            catch (ArgumentException e)
            {
                return controller.Request.CreateErrorResponse(HttpStatusCode.BadRequest, e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="D"></typeparam>
        /// <param name="controller"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static HttpResponseMessage Delete<D>(this GenericController<D> controller, int id)
            where D : class, IDataTransferObject, new()
        {
            HttpStatusCode responseCode = HttpStatusCode.OK;

            if (controller.BaseService.Exists(id))
            {
                try
                {
                    controller.BaseService.Delete(id);
                    responseCode = HttpStatusCode.OK;
                }
                catch (ArgumentException e)
                {
                    return controller.Request.CreateErrorResponse(HttpStatusCode.BadRequest, e);
                }
            }
            else
                responseCode = HttpStatusCode.NotFound;

            return controller.Request.CreateResponse(responseCode);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="ModelState"></param>
        /// <returns></returns>
        public static HttpResponseMessage CreateErrorResponse(this ApiController controller, ModelStateDictionary ModelState)
        {
            string message = ModelState.Values.Where(x => x.Errors.Count != 0).First().Errors[0].ErrorMessage;
            string errorMessage;

            if (!string.IsNullOrEmpty(message))
                errorMessage = message;
            else if (ModelState.Values.Where(x => x.Errors.Count != 0).First().Errors[0].Exception != null)
                errorMessage = ModelState.Values.Where(x => x.Errors.Count != 0).First().Errors[0].Exception.Message;
            else
                errorMessage = "The model state is invalid.";

            return controller.Request.CreateErrorResponse(HttpStatusCode.BadRequest, new Exception(errorMessage));
        }
    }
}