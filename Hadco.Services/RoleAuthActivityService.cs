using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Principal;
using AutoMapper;
using Hadco.Common;
using Hadco.Common.Enums;
using Hadco.Data;
using Hadco.Data.Entities;
using Hadco.Services.DataTransferObjects;

namespace Hadco.Services
{
    public class RoleAuthActivityService : GenericService<RoleAuthActivityDto, RoleAuthActivity>,
        IRoleAuthActivityService
    {
        private readonly IRoleAuthActivityRepository _roleAuthActivityRepository;
        private readonly IRoleService _roleService;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IDepartmentRepository _departmentRepository;

        public RoleAuthActivityService(IRoleService roleService,
            IRoleAuthActivityRepository roleAuthActivityRepository,
            IEmployeeRepository employeeRepository,
            IPrincipal currentUser,
            IDepartmentRepository departmentRepository)
            : base(roleAuthActivityRepository, currentUser)
        {
            _roleAuthActivityRepository = roleAuthActivityRepository;
            _roleService = roleService;
            _employeeRepository = employeeRepository;
            _departmentRepository = departmentRepository;
        }

        // Gets all but employee section authActivities 
        public IEnumerable<int> GetUserActivitiesIds()
        {
            var currentUserRolesIds = _roleService.GetCurrentUsersRoleIds();
            return _roleAuthActivityRepository.All
                .Where(x => currentUserRolesIds.Contains(x.RoleID) &&
                            x.AuthActivity.AuthSectionID != AuthSectionID.Employee)
                .Select(x => x.AuthActivityID)
                .Distinct();
        }

        public IEnumerable<int> GetUserActitiviesByDepartmentId(int departmentId)
        {
            var currentUserRolesIds = _roleService.GetCurrentUsersRoleIds();
            var currentUserDepartmentIds = _employeeRepository
                .GetDepartmentsForEmployee(CurrentUser.GetEmployeeID().Value).Where(x => x.ID == departmentId)
                .Select(y => y.ID).ToList();

            var authActivitiesIds = _roleAuthActivityRepository.All
                .Where(x => currentUserRolesIds.Contains(x.RoleID)
                            && (x.AllDepartments
                                || x.OwnDepartments 
                                && currentUserDepartmentIds.Contains(departmentId)
                                || x.Departments.Any(z => z.ID == departmentId)))
                .Select(x => x.AuthActivityID).Distinct().ToList();

            return authActivitiesIds;
        }

        public RoleAuthActivityDto GetRoleAuthActivityById(int id)
        {
            return Mapper.Map<RoleAuthActivityDto>(_roleAuthActivityRepository.AllIncluding(x => x.Departments)
                .First(x => x.ID == id));
        }

        public IEnumerable<RoleAuthActivityDto> GetRoleAuthActivitiesByActivityIds(IEnumerable<int> activityIds)
        {
            var currentUserRolesIds = _roleService.GetCurrentUsersRoleIds();
            return Mapper.Map<IEnumerable<RoleAuthActivityDto>>(_roleAuthActivityRepository
                .AllIncluding(x => x.Departments)
                .Where(x => currentUserRolesIds.Contains(x.RoleID) && activityIds.Contains(x.AuthActivityID)));
        }

        public IEnumerable<int> GetEditEmployeeAuthActivityIdsForSupervisor()
        {
            return _roleAuthActivityRepository.All
                .Where(x =>
                    (x.Role.Name == ProjectConstants.SupervisorRole ||
                     x.Role.Name == ProjectConstants.SuperintendentRole ||
                     x.Role.Name == ProjectConstants.ManagerRole) &&
                    x.AuthActivity.AuthSectionID == AuthSectionID.Employee).Select(x => x.AuthActivityID).Distinct()
                .ToList();
        }

        public override RoleAuthActivityDto Insert(RoleAuthActivityDto dto)
        {
            var entity = new RoleAuthActivity
            {
                RoleID = dto.RoleID,
                AuthActivityID = (int) dto.AuthActivityID,
                OwnDepartments = dto.OwnDepartments,
                AllDepartments = dto.AllDepartments
            };

            var insertedEntity = _roleAuthActivityRepository.Insert(entity);
            if (dto.DepartmentIds != null)
            {
                var departmentEntities = _departmentRepository.All.Where(x => dto.DepartmentIds.Contains(x.ID));
                _roleAuthActivityRepository.AddRoleAuthActivityDepartments(insertedEntity.ID, dto.DepartmentIds);
                insertedEntity.Departments = departmentEntities.ToList();
            }
            else
            {
                insertedEntity.Departments = new List<Department>();
            }

            return Mapper.Map<RoleAuthActivityDto>(insertedEntity);
        }

        public override RoleAuthActivityDto Update(RoleAuthActivityDto dto)
        {
            var departmentDtos = _departmentRepository.All.Where(x => dto.DepartmentIds.Contains(x.ID));
            var departmentEntities = Mapper.Map<IEnumerable<Department>>(departmentDtos);

            var entity = _roleAuthActivityRepository.All.First(x => x.ID == dto.ID);
            _roleAuthActivityRepository.RemoveRoleAuthActivityDepartments(entity.ID);

            _roleAuthActivityRepository.UpdateRoleAuthActivity(entity.ID, dto.OwnDepartments, dto.AllDepartments);

            _roleAuthActivityRepository.AddRoleAuthActivityDepartments(entity.ID, dto.DepartmentIds);

            entity.RoleID = dto.RoleID;
            entity.AuthActivityID = (int) dto.AuthActivityID;
            entity.OwnDepartments = dto.OwnDepartments;
            entity.AllDepartments = dto.AllDepartments;
            entity.Departments = departmentEntities.ToList();
            return Mapper.Map<RoleAuthActivityDto>(entity);
        }

        public override RoleAuthActivityDto Find(int id, bool tracking = true)
        {
            RoleAuthActivity item;
            if (tracking)
                item = _roleAuthActivityRepository.AllIncluding(x => x.Departments).First(x => x.ID == id);
            else
                item = _roleAuthActivityRepository.AllIncluding(x => x.Departments).AsNoTracking().First(x => x.ID == id);

            return Mapper.Map<RoleAuthActivity, RoleAuthActivityDto>(item);
        }

        public IEnumerable<RoleAuthActivityDto> GetRoleAuthActivitiesByRoleIds()
        {
            var currentUserRoleIds = _roleService.GetCurrentUsersRoleIds();
            var roleAuthActivityDtos = new List<RoleAuthActivityDto>().AsEnumerable();
            var roleAuthActivities = _roleAuthActivityRepository.AllIncluding(x => x.Departments)
                .Where(x => currentUserRoleIds.Contains(x.RoleID)).ToList();
            if (roleAuthActivities.Any())
                roleAuthActivityDtos = Mapper.Map<IEnumerable<RoleAuthActivityDto>>(roleAuthActivities);

            return roleAuthActivityDtos;
        }
    }

    public interface IRoleAuthActivityService : IGenericService<RoleAuthActivityDto>
    {
        IEnumerable<int> GetUserActivitiesIds();
        IEnumerable<int> GetUserActitiviesByDepartmentId(int departmentId);
        RoleAuthActivityDto GetRoleAuthActivityById(int id);
        IEnumerable<RoleAuthActivityDto> GetRoleAuthActivitiesByActivityIds(IEnumerable<int> activityIds);
        IEnumerable<int> GetEditEmployeeAuthActivityIdsForSupervisor();
        IEnumerable<RoleAuthActivityDto> GetRoleAuthActivitiesByRoleIds();
    }
}