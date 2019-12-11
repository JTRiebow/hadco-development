using Hadco.Services;
using Hadco.Services.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.OData;
using Hadco.Web.Models;

namespace Hadco.Web.Controllers
{
	/// <summary>
	/// 
	/// </summary>
    public class RolesController : DefaultController<RoleDto>
    {
		/// <summary>
		/// 
		/// </summary>
		/// <param name="roleService"></param>
		public RolesController(IRoleService roleService) : base(roleService)
		{ }

		#region BASE ENDPOINTS
		/// <summary>
		/// Retrieve a list of roles contained in the system.
		/// </summary>
		/// <returns></returns>
		[EndpointQueryStrings]
		[ResponseType(typeof(PaginatedResult<RoleDto>))]
		public override HttpResponseMessage Get()
		{
			return base.Get();
		}

		/// <summary>
		/// Retrieve a role with the specifiec id.
		/// </summary>
		/// <param name="id">The id of the role being requested.</param>
		/// <returns></returns>
		[ResponseType(typeof(RoleDto))]
		public override HttpResponseMessage Get(int id)
		{
			return base.Get(id);
		}

        /// <summary>
        /// Post a new role into the system.
        /// </summary>
        /// <param name="dto">The new role object that should be added to the system.</param>
        /// <returns></returns>
        [ResponseType(typeof(RoleDto))]
        public override HttpResponseMessage Post(RoleDto dto)
        {
            // return base.Post(dto);
            throw new NotImplementedException(); 
        }

		/// <summary>
		/// Update the role with the given id to have the specified values found in the passed dto.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="dto"></param>
		/// <returns></returns>
		[ResponseType(typeof(RoleDto))]
		public override HttpResponseMessage Put(int id, RoleDto dto)
		{
            // return base.Put(id, dto);
            throw new NotImplementedException(); 
        }

		/// <summary>
		/// Update only the provided fields with the provided values for the role with the given id.
		/// </summary>
		/// <param name="id">Id of the role to be (partially) updated.</param>
		/// <param name="dto">The role object that containd only the fields and values that are t be updated.</param>
		/// <returns></returns>
		[ResponseType(typeof(RoleDto))]
		public override HttpResponseMessage Patch(int id, Delta<RoleDto> dto)
		{
            // base.Patch(id, dto);
            throw new NotImplementedException(); 
		}

        /// <summary>
        /// Remove the requested role, with the given id, from the system.
        /// </summary>
        /// <param name="id">Id value of the role to be removed.</param>
        /// <returns></returns>
        public override HttpResponseMessage Delete(int id)
        {
            //base.Delete(id);
            throw new NotImplementedException(); 
        }

        #endregion
    }
}
