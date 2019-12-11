using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Geocoding;
using Hadco.Common;
using Hadco.Data;
using Hadco.Services.DataTransferObjects;

namespace Hadco.Services
{
    public class SearchService : ISearchService
    {
        private IEmployeeService _employeeService;
        private ITimesheetService _timesheetService;
        private IEmployeeTimecardService _employeeTimecardService;
        public SearchService(IEmployeeService employeeService, ITimesheetService timesheetService, IEmployeeTimecardService employeeTimecardService, IPrincipal currentUser)
        {
            _employeeService = employeeService;
            _timesheetService = timesheetService;
            _employeeTimecardService = employeeTimecardService;
            CurrentUser = (ClaimsPrincipal)currentUser;
        }
        protected ClaimsPrincipal CurrentUser { get; }

        public EmployeeSearchDto Search(int employeeID, DateTime date)
        {
            var result = new EmployeeSearchDto();
            result.Employee = _employeeService.FindExpanded(employeeID);
            result.Timers = _employeeTimecardService.GetTimecardWeeklySummary(date, ProjectConstants.AccountingRole, employeeID: employeeID);
            result.Timesheets = _timesheetService.GetForemanTimesheetsFromEmployee(employeeID, date);
            result.IsAccountingVisible = CurrentUser.IsInRole(ProjectConstants.AccountingRole);
            result.IsBillingVisible = CurrentUser.IsInRole(ProjectConstants.BillingRole);
            result.IsSupervisorVisible = _employeeService.EmployeeSupervisorExists(employeeID, CurrentUser.GetEmployeeID().Value);

            return result;
        }
    }

    public interface ISearchService
    {
        EmployeeSearchDto Search(int employeeID, DateTime date);
    }
}
