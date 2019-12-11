using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using Hadco.Common;
using Hadco.Common.DataTransferObjects;
using Hadco.Common.Enums;
using Hadco.Data;
using Hadco.Services.DataTransferObjects;

namespace Hadco.Services
{
    public class CsvService : ICsvService
    {
        private ICsvRepository _csvRepository;
        private IEmployeeService _employeeService;
        private IRoleAuthActivityService _roleAuthActivityService;
        private IRoleService _roleService;
        private IAuthActivityService _authActivityService;

        public CsvService(ICsvRepository csvRepository, IPrincipal currentUser, IEmployeeService employeeService, IRoleAuthActivityService roleAuthActivityService,
            IRoleService roleService, IAuthActivityService authActivityService)
        {
            _csvRepository = csvRepository;
            CurrentUser = (ClaimsPrincipal)currentUser;
            _employeeService = employeeService;
            _roleAuthActivityService = roleAuthActivityService;
            _roleService = roleService;
            _authActivityService = authActivityService;
        }

        protected ClaimsPrincipal CurrentUser { get; }

        public Csv GetCsv(CsvType type, DateTime startDate, DateTime endDate, DepartmentName[] department)
        {
            VerifyAccess(department);
            var records = _csvRepository.GetCsv(type, startDate, endDate, department);
            return WriteRecords(records, type  + ".csv");
        }

        private void VerifyAccess(DepartmentName[] department)
        {
            var roleAuthActivities = _roleAuthActivityService.GetRoleAuthActivitiesByRoleIds().ToList();
            var allDepartments = RoleAuthActivityHasAllDepartments(roleAuthActivities);
            var currentUserID = (int)CurrentUser.GetEmployeeID();

            foreach (var dept in department)
                if (HasDepartmentID(dept))
                {
                    {
                        var departmentId = (int)dept;
                        var ownDepartments = RoleAuthActivityHasOwnDepartments(departmentId, roleAuthActivities, currentUserID);
                        var roleAuthHasDepartment = RoleAuthActivityDepartmentHasDepartment(departmentId, roleAuthActivities);

                        if (!ownDepartments && !allDepartments && !roleAuthHasDepartment)
                        {
                            throw new UnauthorizedDataAccessException("You are not authorized to view this data");
                        }
                    }
                }
        }

        private bool HasDepartmentID(DepartmentName? department)
        {
            return department.HasValue;
        }

        private bool RoleAuthActivityHasAllDepartments(List<RoleAuthActivityDto> roleAuthActivities)
        {
            var authActivity = _authActivityService.GetAllAuthActivities().Where(x => x.Name.ToLower().Contains("download")).ToList();
            var roleAuth = roleAuthActivities.Where(x => x.AllDepartments == true);
            return authActivity.Any(aa => roleAuth.Any(ra => (int)ra.AuthActivityID == aa.ID));
        }

        private bool RoleAuthActivityHasOwnDepartments(int departmentID, List<RoleAuthActivityDto> roleAuthActivities, int userID)
        {
            var ownDepartment = roleAuthActivities.Exists(x => x.OwnDepartments == true);
            var employeeDepartments = _employeeService.EmployeeDepartmentExists(userID, departmentID);
            return ownDepartment && employeeDepartments;
        }

        private bool RoleAuthActivityDepartmentHasDepartment(int departmentID, List<RoleAuthActivityDto> roleAuthActivities)
        {
            return roleAuthActivities.Exists(x => x.DepartmentIds.Contains(departmentID));
        }

        private Csv WriteRecords(IEnumerable<object> records, string fileName)
        {
            var recordList = records.ToList();
            if (recordList.Count == 0)
            {
                return new Csv() { FileName = fileName };
            }
            using (var stringWriter = new StringWriter())
            {
                using (var csv = new CsvWriter(stringWriter))
                {
                    csv.WriteRecords(recordList);
                    return new Csv()
                    {
                        Data = stringWriter.ToString(),
                        FileName = fileName
                    };
                }
            }
        }
    }

    public interface ICsvService
    {
        Csv GetCsv(CsvType type, DateTime startDate, DateTime endDate, DepartmentName[] department);
    }
}
