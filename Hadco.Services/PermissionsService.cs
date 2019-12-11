using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using Hadco.Common;
using Hadco.Common.Enums;
using Hadco.Data;
using Hadco.Services.DataTransferObjects;

namespace Hadco.Services
{
    public class PermissionsService : IPermissionsService
    {
        private readonly ClaimsPrincipal _currentUser;
        private readonly IRoleAuthActivityService _roleAuthActivityService;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IAuthActivityService _authActivityService;

        public PermissionsService(IPrincipal currentUser, IRoleAuthActivityService roleAuthActivityService,
            IEmployeeRepository employeeRepository, IDepartmentRepository departmentRepository,
            IAuthActivityService authActivityService)
        {
            _currentUser = currentUser as ClaimsPrincipal;
            _roleAuthActivityService = roleAuthActivityService;
            _employeeRepository = employeeRepository;
            _departmentRepository = departmentRepository;
            _authActivityService = authActivityService;
        }

        public IEnumerable<AuthActivityID> GetCurrentUserPermissions()
        {
            if (_currentUser.IsInRole(ProjectConstants.AdminRole))
                return (AuthActivityID[]) Enum.GetValues(typeof(AuthActivityID));

            var permissions = new List<AuthActivityID>();
            if (_currentUser.IsInRole(ProjectConstants.TruckingRole) ||
                _currentUser.IsInRole(ProjectConstants.TruckingReportsRole))
                permissions.Add(AuthActivityID.ViewTruckerDailies);

            if (_currentUser.IsInRole(ProjectConstants.AccountingRole))
                permissions.Add(AuthActivityID.DownloadJobTimersCsv);

            return permissions;
        }

        public IDictionary<string, object> GetAllPermissions()
        {
            return EnumHelper.ToDictionary<AuthActivityID>();
        }

        public IEnumerable<int> GetEmployeeEditPermissionActivityIds(int employeeID)
        {
            var currentUserIsSupervisorOfEmployee = _currentUser.GetEmployeeID().HasValue && _employeeRepository
                                                        .GetDirectSupervisorsForEmployee(employeeID).ToList().Any(x =>
                                                            x.ID == _currentUser.GetEmployeeID().Value);
            if (_currentUser.IsInRole(ProjectConstants.AdminRole)
                || _currentUser.IsInRole(ProjectConstants.HRRole))
                return _authActivityService.GetAuthActivityIdsBySectionID((int) AuthSectionID.Employee);
            if (currentUserIsSupervisorOfEmployee)
                return _roleAuthActivityService.GetEditEmployeeAuthActivityIdsForSupervisor();

            return new List<int>();
        }

        public IEnumerable<int> GetCurrentUserPermissionActivityIds()
        {
            return _roleAuthActivityService.GetUserActivitiesIds();
        }

        public IEnumerable<int> GetCurrentUserPermissionActivityIdsByDepartmentId(int departmentId)
        {
            return _roleAuthActivityService.GetUserActitiviesByDepartmentId(departmentId);
        }

        public IEnumerable<ActivityDepartmentsDto> GetActivityDepartments(IEnumerable<int> activityIds)
        {
            var roleAuthActivityDtos = _roleAuthActivityService.GetRoleAuthActivitiesByActivityIds(activityIds);
            var permissionsForCsvDtos = new List<ActivityDepartmentsDto>();
            var activityIdsSet = new HashSet<int>();
            foreach (var roleAuthActivityDto in roleAuthActivityDtos)
                activityIdsSet.Add((int) roleAuthActivityDto.AuthActivityID);
            foreach (var activityId in activityIdsSet)
            {
                var departmentIds = new List<int>();
                // If any of the roleAuthActivities have AllDepartments set to true give all departments
                if (roleAuthActivityDtos.Any(x => (int) x.AuthActivityID == activityId && x.AllDepartments))
                    departmentIds = _departmentRepository.All.Select(x => x.ID).ToList();
                // If any of the roleAuthActivities have OwnDepartments set grab all departments on the current user
                else if (roleAuthActivityDtos.Any(x => (int) x.AuthActivityID == activityId && x.OwnDepartments) &&
                         _currentUser.GetEmployeeID().HasValue)
                    departmentIds = _employeeRepository.GetDepartmentsForEmployee(_currentUser.GetEmployeeID().Value)
                        .Select(x => x.ID).ToList();
                // Grab all of the departments on the roleAuthActivities DepartmentIds list
                //if (roleAuthActivityDtos.Any(x => (int) x.AuthActivityID == activityId))
                //{
                    departmentIds.AddRange(roleAuthActivityDtos.Where(x => (int) x.AuthActivityID == activityId)
                        .SelectMany(x => x.DepartmentIds).Distinct().ToList());
                //}

                permissionsForCsvDtos.Add(new ActivityDepartmentsDto
                {
                    ID = activityId,
                    DepartmentIds = departmentIds
                });
            }

            return permissionsForCsvDtos;
        }

        public bool HasPermission(AuthActivityID authActivityID, int departmentID)
        {
            var activities = _roleAuthActivityService.GetUserActitiviesByDepartmentId(departmentID);
            return activities.Contains((int)authActivityID);
        }

        public void CheckPermission(AuthActivityID authActivityID, int departmentID)
        {
            var activities = _roleAuthActivityService.GetUserActitiviesByDepartmentId(departmentID);
            if (!activities.Contains((int)authActivityID))
            {
                throw new UnauthorizedAccessException("Employee does not have the correct permission for this action");
            }
        }
    }

    public interface IPermissionsService
    {
        IEnumerable<AuthActivityID> GetCurrentUserPermissions();
        IDictionary<string, object> GetAllPermissions();
        IEnumerable<int> GetCurrentUserPermissionActivityIds();
        IEnumerable<int> GetCurrentUserPermissionActivityIdsByDepartmentId(int departmentId);
        IEnumerable<ActivityDepartmentsDto> GetActivityDepartments(IEnumerable<int> activityIds);
        IEnumerable<int> GetEmployeeEditPermissionActivityIds(int employeeID);
        bool HasPermission(AuthActivityID authActivityID, int departmentID);
        void CheckPermission(AuthActivityID authActivityID, int departmentID);
    }
}