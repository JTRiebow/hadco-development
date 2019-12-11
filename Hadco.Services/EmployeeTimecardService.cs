using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using System.Security.Principal;
using Hadco.Data;
using Hadco.Services.DataTransferObjects;
using Hadco.Data.Entities;
using Hadco.Common;
using Hadco.Common.DataTransferObjects;
using CsvHelper;
using System.IO;
using Hadco.Common.Enums;

namespace Hadco.Services
{
    public class EmployeeTimecardService : GenericService<EmployeeTimecardDto, EmployeeTimecard>,
        IEmployeeTimecardService
    {
        private IDepartmentService _departmentService;
        private IEmployeeTimecardRepository _employeeTimecardRepository;

        public EmployeeTimecardService(IEmployeeTimecardRepository employeeTimeCardRepository,
            IDepartmentService departmentService, IPrincipal currentUser)
            : base(employeeTimeCardRepository, currentUser)
        {
            _employeeTimecardRepository = employeeTimeCardRepository;
            _departmentService = departmentService;
        }

        public IEnumerable<TimecardWeeklySummaryDto> GetTimecardWeeklySummary(DateTime week, string role,
            int? supervisorID = null, int? departmentID = null, int? employeeID = null, bool? accountingApproved = null,
            bool? supervisorApproved = null, bool? billingApproved = null)
        {
            var currentUserId = CurrentUser.GetEmployeeID() ?? -1;

            var supervisorViewPermissionKey = "ViewSupervisorTimers";
            var billingViewPermissionKey = "ViewBillingTimers";
            var accountingViewPermissionKey = "ViewAccountingTimers";

            if (role == ProjectConstants.AccountingRole)
            {
                return _employeeTimecardRepository.GetTimecardWeeklySummary(week, currentUserId, null, departmentID, employeeID,
                    accountingViewPermissionKey, accountingApproved, supervisorApproved, billingApproved);
            }
            if (role == ProjectConstants.SupervisorRole || role == ProjectConstants.ManagerRole)
            {
                return _employeeTimecardRepository.GetTimecardWeeklySummary(week, currentUserId, CurrentUser.GetEmployeeID(),
                    departmentID, employeeID, supervisorViewPermissionKey, accountingApproved, supervisorApproved, billingApproved);
            }
            if (role == ProjectConstants.BillingRole)
            {
                return _employeeTimecardRepository.GetTimecardWeeklySummary(week, currentUserId, null,
                   departmentID, employeeID, billingViewPermissionKey, accountingApproved, supervisorApproved, billingApproved);
            }
            throw new UnauthorizedDataAccessException("You must be a supervisor or accountant to view this data.");
        }

        public IEnumerable<EmployeeTimecardSummaryDto> GetCurrentUserTimecardSummary(DateTime week)
        {
            var currentUserID = CurrentUser.GetEmployeeID();
            if (currentUserID.HasValue)
            {
                return _employeeTimecardRepository.GetEmployeeTimecardSummary(week, currentUserID.Value);
            }
            else return new List<EmployeeTimecardSummaryDto>();
        }

        public EmployeeTimecardDto GetOrCreateTimecard(int employeeID, DateTime day, int departmentID)
        {
            var startOfWeek = day.StartOfWeek(DayOfWeek.Sunday).Date;
            var timecard =
                _employeeTimecardRepository.All.FirstOrDefault(
                    x =>
                        x.EmployeeID == employeeID && x.DepartmentID == departmentID &&
                        x.StartOfWeek == startOfWeek);
            if (timecard == null)
            {
                EmployeeTimecard et = new EmployeeTimecard()
                {
                    EmployeeID = employeeID,
                    DepartmentID = departmentID,
                    SubDepartmentID =
                        _departmentService.GetSubDepartment(employeeID, departmentID),
                    StartOfWeek = startOfWeek
                };
                timecard = _employeeTimecardRepository.Insert(et);
            }
            return Mapper.Map<EmployeeTimecardDto>(timecard);
        }

        public override EmployeeTimecardDto Insert(EmployeeTimecardDto dto)
        {
            dto.SubDepartmentID = _departmentService.GetSubDepartment(dto.EmployeeID, dto.DepartmentID);
            return base.Insert(dto);
        }
    }

    public interface IEmployeeTimecardService : IGenericService<EmployeeTimecardDto>
    {
        EmployeeTimecardDto GetOrCreateTimecard(int employeeID, DateTime day, int departmentID);

        IEnumerable<TimecardWeeklySummaryDto> GetTimecardWeeklySummary(DateTime week, string role,
            int? supervisorID = null, int? departmentID = null, int? employeeID = null, bool? accountingApproved = null,
            bool? supervisorApproved = null, bool? billingApproved = null);

        IEnumerable<EmployeeTimecardSummaryDto> GetCurrentUserTimecardSummary(DateTime week);
    }
}