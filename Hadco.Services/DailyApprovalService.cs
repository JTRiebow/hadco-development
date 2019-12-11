using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using Hadco.Common.DataTransferObjects;
using Hadco.Common.Enums;
using Hadco.Data;
using Hadco.Data.Entities;
using Hadco.Services.DataTransferObjects;
using System.Security.Principal;
using System.Web.Http.OData;
using AutoMapper;
using Hadco.Common;

namespace Hadco.Services
{
    public class DailyApprovalService : GenericService<DailyApprovalDto, DailyApproval>, IDailyApprovalService
    {
        private IDailyApprovalRepository _dailyApprovalRepository;
        private IEmployeeService _employeeService;
        private IPermissionsService _permissionService;

        public DailyApprovalService(IDailyApprovalRepository dailyApprovalRepository,
            IEmployeeService employeeService,
            IPermissionsService permissionService,
            IPrincipal currentUser)
            : base(dailyApprovalRepository, currentUser)
        {
            _dailyApprovalRepository = dailyApprovalRepository;
            _employeeService = employeeService;
            _permissionService = permissionService;
        }

        public BaseDailyApprovalDto Get(int employeeID, DateTime day, int departmentID)
        {
            DailyApproval approval;
            var approvals = _dailyApprovalRepository.All.Where(x => x.EmployeeID == employeeID && x.Day == day && x.DepartmentID == departmentID);
            if (approvals.Any())
            {
                approval = approvals.Single();
            }
            else
            {
                approval = _dailyApprovalRepository.Insert(
                    new DailyApproval() { EmployeeID = employeeID, Day = day, DepartmentID = departmentID });
            }
            return AutoMapper.Mapper.Map<BaseDailyApprovalDto>(approval);
        }

        public BaseDailyApprovalDto Update(int id, Delta<DailyApprovalPatchDto> dto)
        {
            var currentApproval = _dailyApprovalRepository.Find(id);
            return Update(currentApproval, dto);
        }

        public BaseDailyApprovalDto Update(int employeeID, DateTime day, int departmentID, Delta<DailyApprovalPatchDto> dto)
        {
            var currentApproval =
                _dailyApprovalRepository.All.FirstOrDefault(
                    x => x.EmployeeID == employeeID && x.Day == day && x.DepartmentID == departmentID);
            return Update(currentApproval, dto);
        }


        public BaseDailyApprovalDto GetOrCreateDailyApproval(int employeeID, DateTime day, int departmentID)
        {
            var dailyApproval = Get(employeeID, day, departmentID);

            if (dailyApproval == null)
            {
                dailyApproval = Insert(new DailyApprovalDto()
                                             {
                                                 EmployeeID = employeeID,
                                                 Day = day,
                                                 DepartmentID = departmentID
                                             });
            }

            return dailyApproval;
        }

        private BaseDailyApprovalDto Update(DailyApproval currentApproval, Delta<DailyApprovalPatchDto> dto)
        {
            var approvalEmployeeID = currentApproval.EmployeeID;
            var currentEmployeeID = CurrentUser.GetEmployeeID().Value;

            var newApproval = Mapper.Map<DailyApprovalPatchDto>(currentApproval);
            dto.Patch(newApproval);

            var supervisorChanged = newApproval.ApprovedBySupervisor != currentApproval.ApprovedBySupervisor;
            var billingApprovalChanged = newApproval.ApprovedByBilling != currentApproval.ApprovedByBilling;
            var accountingApprovalChanged = newApproval.ApprovedByAccounting != currentApproval.ApprovedByAccounting;

            var attemptingToApproveAsSupervisor = supervisorChanged && newApproval.ApprovedBySupervisor;
            var attemptingToApproveAsBilling = billingApprovalChanged && newApproval.ApprovedByBilling;
            var attemptingToApproveAsAccounting = accountingApprovalChanged && newApproval.ApprovedByAccounting;

            var attemptingToRejectSupervisor = supervisorChanged && !newApproval.ApprovedBySupervisor;
            var attemptingToRejectBilling = billingApprovalChanged && !newApproval.ApprovedByBilling;
            var attemptingRejectAccounting = accountingApprovalChanged && !newApproval.ApprovedByAccounting;

            if (attemptingToApproveAsSupervisor)
            {
                var permitted = CanApproveSupervisor(approvalEmployeeID, currentApproval.DepartmentID);
                if (permitted)
                {
                    currentApproval.ApprovedBySupervisor = true;
                    currentApproval.ApprovedBySupervisorEmployeeID = currentEmployeeID;
                    currentApproval.ApprovedBySupervisorTime = DateTimeOffset.Now;

                    if (currentApproval.DepartmentID == (int?)DepartmentName.Mechanic || 
                        currentApproval.DepartmentID == (int?)DepartmentName.FrontOffice || 
                        currentApproval.DepartmentID == (int?)DepartmentName.TMCrushing)
                    {
                        currentApproval.ApprovedByBilling = true;
                        currentApproval.ApprovedByBillingEmployeeID = currentEmployeeID;
                        currentApproval.ApprovedByBillingTime = DateTimeOffset.Now;
                    }
                }
                else
                {
                    throw new Exception("User does not have rights to approve as supervisor");
                }
            }
            if (attemptingToApproveAsBilling)
            {
                var permitted = _permissionService.HasPermission(AuthActivityID.ApproveAsBilling, currentApproval.DepartmentID);
                if (permitted)
                {
                    currentApproval.ApprovedByBilling = true;
                    currentApproval.ApprovedByBillingEmployeeID = currentEmployeeID;
                    currentApproval.ApprovedByBillingTime = DateTimeOffset.Now;
                }
                else
                {
                    throw new Exception("User does not have rights to approve as billing");
                }
            }
            if (attemptingToApproveAsAccounting)
            {
                var permitted = _permissionService.HasPermission(AuthActivityID.ApproveAsAccounting, currentApproval.DepartmentID);
                if (permitted)
                {
                    currentApproval.ApprovedByAccounting = true;
                    currentApproval.ApprovedByAccountingEmployeeID = currentEmployeeID;
                    currentApproval.ApprovedByAccountingTime = DateTimeOffset.Now;
                }
                else
                {
                    throw new Exception("User does not have rights to approve as accounting");
                }
            }
            if (attemptingToRejectSupervisor)
            {
                var permitted = CanUnapproveSupervisor(approvalEmployeeID, currentApproval.DepartmentID);
                if (permitted)
                {
                    currentApproval.ApprovedBySupervisor = false;
                }
                else
                {
                    throw new Exception("User does not have rights to reject supervisor approvals");
                }
            }
            if (attemptingToRejectBilling)
            {
                var permitted = _permissionService.HasPermission(AuthActivityID.RejectBillingApproval, currentApproval.DepartmentID);
                if (permitted)
                {
                    currentApproval.ApprovedByBilling = false;
                }
                else
                {
                    throw new Exception("User does not have rights to reject billing approvals");
                }
            }
            if (attemptingRejectAccounting)
            {
                var permitted = _permissionService.HasPermission(AuthActivityID.RejectAccountingApproval, currentApproval.DepartmentID);
                if (permitted)
                {
                    currentApproval.ApprovedByAccounting = false;
                }
                else
                {
                    throw new Exception("User does not have rights to reject accounting approvals");
                }
            }

            return Mapper.Map<BaseDailyApprovalDto>(base.Update(Mapper.Map<DailyApprovalDto>(currentApproval)));
        }

        private bool CanApproveSupervisor(int employeeID, int departmentID)
        {
            var permitted = _permissionService.HasPermission(AuthActivityID.ApproveAsSupervisor, departmentID);
            if (permitted)
            {
                return true;
            }
            var isDirectSupervisor = _employeeService.EmployeeSupervisorExists(employeeID, CurrentUser.GetEmployeeID().Value);
            if (isDirectSupervisor)
            {
                return true;
            }
            return false;
        }

        private bool CanUnapproveSupervisor(int employeeID, int departmentID)
        {
            var permitted = _permissionService.HasPermission(AuthActivityID.RejectSupervisorApproval, departmentID);
            if (permitted)
            {
                return true;
            }
            var isDirectSupervisor = _employeeService.EmployeeSupervisorExists(employeeID, CurrentUser.GetEmployeeID().Value);
            if (isDirectSupervisor)
            {
                return true;
            }
            return false;
        }
    }

    public interface IDailyApprovalService : IGenericService<DailyApprovalDto>
    {
        BaseDailyApprovalDto Get(int employeeID, DateTime day, int departmentID);
        BaseDailyApprovalDto Update(int id, Delta<DailyApprovalPatchDto> dto);
        BaseDailyApprovalDto Update(int employeeID, DateTime day, int departmentID, Delta<DailyApprovalPatchDto> dto);
        BaseDailyApprovalDto GetOrCreateDailyApproval(int dtoEmployeeID, DateTime dtoDay, int dtoDepartmentID);
    }
}
