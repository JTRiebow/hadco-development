using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using System.Security.Claims;
using System.Security.Principal;
using Hadco.Data;
using Hadco.Services.DataTransferObjects;
using Hadco.Data.Entities;
using Hadco.Common;

namespace Hadco.Services
{
	public class RoleService : GenericService<RoleDto, Role>, IRoleService
	{
	    private IRoleRepository _roleRepository;
        public RoleService(IRoleRepository roleRepository, IPrincipal currentUser)
            : base(roleRepository, currentUser)
		{
			_roleRepository = roleRepository;
		}

		#region RoleResources

        public IEnumerable<RoleResourceDto> GetAllRoleResourceDto()
        {
            return Mapper.Map<IEnumerable<RoleResourceDto>>(_roleRepository.GetAllRoleResources());
        }

		public RoleResourceDto AddResource(int id, RoleResourceDto dto)
		{
			return Mapper.Map<RoleResourceDto>(_roleRepository.AddResource(id, Mapper.Map<RoleResource>(dto)));
		}

		public void RemoveResource(int id, int resourceID)
		{
			_roleRepository.RemoveResource(id, resourceID);
		}

		public bool RoleResourceExists(int id, int resourceID)
		{
			return _roleRepository.RoleResourceExists(id, resourceID);
		}

		public RoleResourceDto UpdateResourcePermissionForRole(int id, RoleResourceDto dto)
		{
			return Mapper.Map<RoleResourceDto>(_roleRepository.UpdateResourcePermissionForRole(id, Mapper.Map<RoleResource>(dto)));
		}

        public IEnumerable<int> GetCurrentUsersRoleIds()
        {
            var currentUserRoles = CurrentUser.GetRoles();
            return _roleRepository.All.Where(x => currentUserRoles.Contains(x.Name)).Select(x => x.ID).ToList();
        }
		#endregion
	}

	public interface IRoleService : IGenericService<RoleDto>
	{
        IEnumerable<RoleResourceDto> GetAllRoleResourceDto();
		RoleResourceDto AddResource(int id, RoleResourceDto dto);
		void RemoveResource(int id, int resourceID);
		bool RoleResourceExists(int id, int resourceID);
		RoleResourceDto UpdateResourcePermissionForRole(int id, RoleResourceDto dto);
        IEnumerable<int> GetCurrentUsersRoleIds();

    }
}
