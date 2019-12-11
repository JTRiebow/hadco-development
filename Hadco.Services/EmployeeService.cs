using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using System.Security.Principal;
using Hadco.Data;
using Hadco.Services.DataTransferObjects;
using Hadco.Data.Entities;
using Hadco.Common;
using System.Collections.Specialized;
using Hadco.Common.DataTransferObjects;
using Microsoft.Data.OData.Query.SemanticAst;

namespace Hadco.Services
{
    public class EmployeeService : GenericService<EmployeeDto, Employee>, IEmployeeService
    {
        private IEmployeeRepository _employeeRepository;
        private ITimesheetRepository _timesheetRepository;
        private IPermissionsService _permissionsService;
        private IRoleService _roleService;
        public EmployeeService(IEmployeeRepository employeeRepository, ITimesheetRepository timesheetRepository, IRoleService roleService, IPermissionsService permissionsService, IPrincipal currentUser)
            : base(employeeRepository, currentUser)
        {
            _employeeRepository = employeeRepository;
            _timesheetRepository = timesheetRepository;
            _roleService = roleService;
            _permissionsService = permissionsService;
        }

        public ExpandedEmployeeDto FindExpanded(int id)
        {
            return Mapper.Map<ExpandedEmployeeDto>(_employeeRepository.Find(id, x => x.Departments, x => x.Roles, x => x.Supervisors, x => x.ClockedInStatus));
        }

        public IEnumerable<EmployeeDto> GetAllSupervisors(NameValueCollection filter, Pagination pagination, out int resultCount, out int totalResultCount)
        {
            IQueryable<Employee> query;
            if (filter != null)
            {
                query = _employeeRepository.AllIncluding(x => x.Roles).Filter(filter);
            }
            else
            {
                query = _employeeRepository.AllIncluding(x => x.Roles);
            }

            query = query.Where(x => x.Roles.Any(y => y.Name == ProjectConstants.SupervisorRole || y.Name == ProjectConstants.ManagerRole));

            // if there is pagination then run the query twice, else convert to a list to get the count so that the query only runs once.
            List<Employee> result;
            if (pagination != null)
            {
                totalResultCount = query.Count();
                result = query.Pagination(pagination).ToList();
                resultCount = result.Count;
            }
            else
            {
                result = query.ToList();
                resultCount = result.Count;
                totalResultCount = resultCount;
            }

            return Mapper.Map<IEnumerable<EmployeeDto>>(result);
        }

        public IEnumerable<ExpandedEmployeeDto> GetAllExpanded(int? supervisorID, NameValueCollection filter, Pagination pagination, out int resultCount, out int totalResultCount)
        {
            var currentUserId = CurrentUser.GetEmployeeID() ?? -1;

            var employeeListings = _employeeRepository.GetEmployeeListings(currentUserId);

            var query = filter != null ? employeeListings.Filter(filter) : employeeListings;

            // if there is pagination then run the query twice, else convert to a list to get the count so that the query only runs once.
            List<Employee> result;
            if (pagination != null)
            {
                totalResultCount = query.Count();
                result = query.Pagination(pagination).ToList();
                resultCount = result.Count;
            }
            else
            {
                result = query.ToList();
                resultCount = result.Count;
                totalResultCount = resultCount;
            }

            return Mapper.Map<IEnumerable<ExpandedEmployeeDto>>(result);
        }

        public PatchPostEmployeeDto FindPatch(int id)
        {
            return Mapper.Map<PatchPostEmployeeDto>(_employeeRepository.FindNoTracking(id));
        }

        public bool IsValidPin(int employeeID, string pin, out bool supervisorApproved)
        {
            supervisorApproved = false;
            var employee = _employeeRepository.AllIncluding(x => x.Supervisors).FirstOrDefault(x => x.ID == employeeID);
            if (employee == null) return false;

            if (employee.Pin != null && employee.Pin.ToLowerInvariant() == pin.ToLowerInvariant())
            {
                return true;
            }
            if (employee.Supervisors.Select(x => x.Pin).Contains(pin))
            {
                supervisorApproved = true;
                return true;
            }
            return false;
        }

        public bool IsValidPin(int employeeID, string pin, int timesheetID, out bool supervisorApproved)
        {
            var timesheet = _timesheetRepository.AllIncluding(x => x.Employee.Roles, x => x.Employee.Departments).SingleOrDefault(x => x.ID == timesheetID);
            var supervisor = timesheet?.Employee;
            if (supervisor != null
                && supervisor.Departments.Select(x => x.ID).Contains(timesheet.DepartmentID)
                && (supervisor.Roles.Select(x => x.Name).Intersect(new []
            {
                ProjectConstants.SupervisorRole,
                ProjectConstants.ManagerRole,
                ProjectConstants.ForemanRole,
                ProjectConstants.SuperintendentRole
            }).Any()
                && supervisor.Pin != null
                && supervisor.Pin.ToLowerInvariant() == pin.ToLowerInvariant()))
            {
                supervisorApproved = true;
                return true;
            }
            if (IsValidPin(employeeID, pin, out supervisorApproved) && !supervisorApproved)
            {
                return true;
            }
            supervisorApproved = false;
            return false;
        }

        public EmployeeDto Update(PatchPostEmployeeDto dto)
        {
            Employee entity = _employeeRepository.Find(dto.ID);
            if (entity.Password != dto.Password)
            {
                dto.Password = PasswordHash.CreateHash(dto.Password);
            }
            Mapper.Map(dto, entity);
            _employeeRepository.Save();
            return Mapper.Map<EmployeeDto>(entity);
        }

        public virtual EmployeeDto Insert(PatchPostEmployeeDto dto)
        {
            dto.Password = PasswordHash.CreateHash(dto.Password);
            var employee = Mapper.Map<Employee>(dto);
            employee.OriginComputerEase = false;
            employee.EmployeeTypeID = (int)EmployeeTypeEnum.FullTime;
            employee.Status = EntityStatus.Active;
            return Mapper.Map<EmployeeDto>(_employeeRepository.Insert(employee));
        }

        public bool IsLoggedIn(int employeeID)
        {
            return _employeeRepository.IsLoggedIn(employeeID);
        }

        #region Roles
        public IEnumerable<RoleDto> GetRolesForUser(int id, Pagination pagination, out int resultCount, out int totalResultCount)
        {
            IQueryable<Role> query;
            List<Role> result;
            query = _employeeRepository.GetRolesForUser(id);

            if (pagination != null)
            {
                totalResultCount = query.Count();
                result = query.Pagination<Role>(pagination).ToList<Role>();
                resultCount = result.Count;
            }
            else
            {
                result = query.ToList<Role>();
                resultCount = result.Count;
                totalResultCount = resultCount;
            }
            return Mapper.Map<IEnumerable<RoleDto>>(result);
        }

        public RoleDto AddRole(int id, RoleDto dto)
        {
            return Mapper.Map<RoleDto>(_employeeRepository.AddRole(id, Mapper.Map<Role>(dto)));
        }

        public void RemoveRole(int id, int roleID)
        {
            _employeeRepository.RemoveRole(id, roleID);
        }

        public bool EmployeeRoleExists(int id, int roleID)
        {
            return _employeeRepository.EmployeeRoleExists(id, roleID);
        }

        public bool IsEmployeeSuperintendent(int employeeID)
        {
            return _employeeRepository.IsEmployeeSuperintendent(employeeID);
        }
        #endregion Roles

        #region Departments
        public IEnumerable<DepartmentDto> GetDepartmentsForEmployee(int employeeID, Pagination pagination, out int resultCount, out int totalResultCount)
        {
            IQueryable<Department> query;
            List<Department> result;
            query = _employeeRepository.GetDepartmentsForEmployee(employeeID);

            if (pagination != null)
            {
                totalResultCount = query.Count();
                result = query.Pagination<Department>(pagination).ToList<Department>();
                resultCount = result.Count;
            }
            else
            {
                result = query.ToList<Department>();
                resultCount = result.Count;
                totalResultCount = resultCount;
            }
            return Mapper.Map<IEnumerable<DepartmentDto>>(result);
        }

        public DepartmentDto AddDepartment(int employeeID, DepartmentDto dto)
        {
            return Mapper.Map<DepartmentDto>(_employeeRepository.AddDepartment(employeeID, Mapper.Map<Department>(dto)));
        }

        public void RemoveDepartment(int employeeID, int departmentID)
        {
            _employeeRepository.RemoveDepartment(employeeID, departmentID);
        }

        public bool EmployeeDepartmentExists(int employeeID, int departmentID)
        {
            return 0 < _employeeRepository.AllIncluding(x => x.Departments).Where(x => x.ID == employeeID && x.Departments.Any(y => y.ID == departmentID)).Count();
        }
        #endregion Departments

        #region Supervisors
        public IEnumerable<EmployeeDto> GetSupervisorsForEmployee(int employeeID, Pagination pagination, out int resultCount, out int totalResultCount)
        {
            IQueryable<Employee> query = _employeeRepository.GetDirectSupervisorsForEmployee(employeeID);

            List<Employee> result;
            if (pagination != null)
            {
                totalResultCount = query.Count();
                result = query.Pagination(pagination).ToList();
                resultCount = result.Count;
            }
            else
            {
                result = query.ToList();
                resultCount = result.Count;
                totalResultCount = resultCount;
            }
            return Mapper.Map<IEnumerable<EmployeeDto>>(result);
        }

        public IEnumerable<EmployeeDailyTimesheetDto> GetDailySupervisorsList(DateTime day, int departmentID, int employeeID)
        {
            return _employeeRepository.GetDailySupervisorsList(day, departmentID, employeeID);
        }

        public EmployeeDto AddSupervisor(int employeeID, EmployeeDto dto)
        {
            return Mapper.Map<EmployeeDto>(_employeeRepository.AddSupervisor(employeeID, Mapper.Map<Employee>(dto)));
        }

        public void RemoveSupervisor(int employeeID, int supervisorID)
        {
            _employeeRepository.RemoveSupervisor(employeeID, supervisorID);
        }

        public bool EmployeeSupervisorExists(int employeeID, int supervisorID)
        {
            return _employeeRepository.AllIncluding(x => x.Supervisors).Any(x => x.ID == employeeID && x.Supervisors.Any(y => y.ID == supervisorID));
        }
        #endregion Supervisors
    }

    public interface IEmployeeService : IGenericService<EmployeeDto>
    {
        EmployeeDto Insert(PatchPostEmployeeDto dto);
        ExpandedEmployeeDto FindExpanded(int id);
        IEnumerable<ExpandedEmployeeDto> GetAllExpanded(int? supervisorID, NameValueCollection filter, Pagination pagination, out int resultCount, out int totalResultCount);
        IEnumerable<EmployeeDto> GetAllSupervisors(NameValueCollection filter, Pagination pagination, out int resultCount, out int totalResultCount);
        PatchPostEmployeeDto FindPatch(int id);
        EmployeeDto Update(PatchPostEmployeeDto dto);
        bool IsValidPin(int employeeID, string pin, out bool supervisorApproved);
        bool IsValidPin(int employeeID, string pin, int timesheetID, out bool supervisorApproved);
        IEnumerable<RoleDto> GetRolesForUser(int id, Pagination pagination, out int resultCount, out int totalResultCount);
        RoleDto AddRole(int id, RoleDto dto);
        void RemoveRole(int id, int roleID);
        bool EmployeeRoleExists(int id, int roleID);
        bool IsEmployeeSuperintendent(int employeeID);
        bool IsLoggedIn(int employeeID);

        #region Departments
        IEnumerable<DepartmentDto> GetDepartmentsForEmployee(int employeeID, Pagination pagination, out int resultCount, out int totalResultCount);
        DepartmentDto AddDepartment(int employeeID, DepartmentDto dto);
        void RemoveDepartment(int employeeID, int departmentID);
        bool EmployeeDepartmentExists(int employeeID, int departmentID);
        #endregion Departments

        #region Supervisors
        IEnumerable<EmployeeDto> GetSupervisorsForEmployee(int employeeID, Pagination pagination, out int resultCount, out int totalResultCount);

        IEnumerable<EmployeeDailyTimesheetDto> GetDailySupervisorsList(DateTime day, int departmentID, int employeeID);
        EmployeeDto AddSupervisor(int employeeID, EmployeeDto dto);
        void RemoveSupervisor(int employeeID, int supervisorID);
        bool EmployeeSupervisorExists(int employeeID, int supervisorID);
        #endregion Supervisors
    }
}
