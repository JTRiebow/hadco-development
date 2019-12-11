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
using System.Runtime.CompilerServices;
using System.Transactions;
using Geocoding;
using Hadco.Common.DataTransferObjects;
using Hadco.Common.Enums;
using Hadco.Data.Migrations;

namespace Hadco.Services
{
    public class EmployeeTimerService : GenericService<EmployeeTimerDto, EmployeeTimer>, IEmployeeTimerService
    {
        private IEmployeeTimerRepository _employeeTimerRepository;
        private IEmployeeTimecardService _employeeTimecardService;
        private ITimesheetService _timesheetService;
        private INoteRepository _noteRepository;
        private IDepartmentService _departmentService;
        IDailyApprovalService _dailyApprovalService;
        public EmployeeTimerService(IEmployeeTimerRepository employeeTimerRepository,
                                    IEmployeeTimecardService employeeTimecardService,
                                    ITimesheetService timesheetService,
                                    INoteRepository noteRepository,
                                    IDepartmentService departmentService,
                                    IDailyApprovalService dailyApprovalService,
                                    IPrincipal currentUser)
            : base(employeeTimerRepository, currentUser)
        {
            _employeeTimerRepository = employeeTimerRepository;
            _employeeTimecardService = employeeTimecardService;
            _timesheetService = timesheetService;
            _noteRepository = noteRepository;
            _departmentService = departmentService;
            _dailyApprovalService = dailyApprovalService;
        }

        public bool Exists(int employeeID, DateTime day, int departmentID)
        {
            var exists = _employeeTimerRepository.All.Any(x => x.EmployeeID == employeeID && x.Day == day.Date && x.DepartmentID == departmentID);
            return exists;
        }

        public new EmployeeTimerDto Insert(EmployeeTimerDto dto)
        {
            using (var scope = new TransactionScope())
            {
                if (!dto.EmployeeTimecardID.HasValue)
                {
                    var etc = _employeeTimecardService.GetOrCreateTimecard(dto.EmployeeID, dto.Day, dto.DepartmentID);
                    dto.EmployeeTimecardID = etc.ID;
                }
                if (!HasValidTimesheetID(dto))
                {
                    dto.TimesheetID = null;
                }
                if (!dto.TimesheetID.HasValue && DepartmentGroups.EmployeeTimerRequiresTimesheet(dto.DepartmentID))
                {
                    var ts = _timesheetService.GetOrCreateTimesheet(dto.EmployeeID, dto.Day, dto.DepartmentID);
                    dto.TimesheetID = ts.ID;
                }
                dto.SubDepartmentID = _departmentService.GetSubDepartment(dto.EmployeeID, dto.DepartmentID);

                var dailyApproval = _dailyApprovalService.GetOrCreateDailyApproval(dto.EmployeeID, dto.Day, dto.DepartmentID);


                var result = base.Insert(dto);
                if (result.Injured)
                {
                    _noteRepository.TriggerFlag(NoteTypeName.Injury,
                                                dto.EmployeeID,
                                                dto.Day,
                                                dto.DepartmentID,
                                                CurrentUser.GetEmployeeID() ?? 1);
                }
                scope.Complete();
                return result;
            }
        }

        public override EmployeeTimerDto Update(EmployeeTimerDto dto)
        {
            var employeeTimerDto = new EmployeeTimerDto();

            employeeTimerDto = base.Update(dto);
            _noteRepository.TriggerFlag(NoteTypeName.Injury, employeeTimerDto.EmployeeID, employeeTimerDto.Day, employeeTimerDto.DepartmentID, CurrentUser.GetEmployeeID().Value);
            return employeeTimerDto;
        }

        public ExpandedEmployeeTimerDto FindExpanded(int employeeID, DateTime day, int departmentID)
        {
            var newDay = day.Date;
            var employeeTimers = _employeeTimerRepository
            .AllIncluding(x => x.EmployeeTimerEntries.Select(y => y.EmployeeTimerEntryHistories.Select(z => z.ChangedBy)),
                                              x => x.EmployeeTimecard,
                                              x => x.EmployeeJobTimers.Select(y => y.EmployeeJobEquipmentTimers.Select(z => z.Equipment)),
                                              x => x.Employee,
                                              x => x.AuthorizeNote,
                                              x => x.Equipment,
                                              x => x.Occurrences);

            var employeeTimer = employeeTimers.FirstOrDefault(x => x.EmployeeID == employeeID && x.Day == newDay && x.DepartmentID == departmentID);
            return Mapper.Map<ExpandedEmployeeTimerDto>(employeeTimer);
        }

        public ExpandedEmployeeTimerDto FindExpanded(int employeeID, DateTime day, int departmentID, int supervisorID)
        {
            var newDay = day.Date;
            var employeeTimers = _employeeTimerRepository
                                        .AllIncluding(x => x.EmployeeTimerEntries.Select(y => y.EmployeeTimerEntryHistories.Select(z => z.ChangedBy)),
                                              x => x.EmployeeTimecard,
                                              x => x.EmployeeJobTimers.Select(y => y.EmployeeJobEquipmentTimers.Select(z => z.Equipment)),
                                              x => x.Employee,
                                              x => x.AuthorizeNote,
                                              x => x.Equipment,
                                              x => x.Occurrences,
                                              x => x.Timesheet);

            var employeeTimer = employeeTimers.SingleOrDefault(x => x.EmployeeID == employeeID &&
                                            x.Day == newDay &&
                                            x.DepartmentID == departmentID &&
                                            x.Timesheet.EmployeeID == supervisorID);

            return Mapper.Map<ExpandedEmployeeTimerDto>(employeeTimer);
        }

        public IEnumerable<ExpandedEmployeeTimerDto> GetAllExpanded(NameValueCollection filter, Pagination pagination, out int resultCount, out int totalResultCount)
        {
            IQueryable<EmployeeTimer> query;
            if (filter != null)
            {
                query = _employeeTimerRepository.AllIncluding(x => x.EmployeeTimerEntries.Select(y => y.EmployeeTimerEntryHistories.Select(z => z.ChangedBy)),
                                              x => x.EmployeeJobTimers.Select(y => y.EmployeeJobEquipmentTimers),
                                              x => x.Employee,
                                              x => x.Equipment,
                                              x => x.Timesheet.Employee,
                                              x => x.AuthorizeNote,
                                              x => x.Occurrences).Filter(filter);
            }
            else
            {
                query = _employeeTimerRepository.AllIncluding(x => x.EmployeeTimerEntries.Select(y => y.EmployeeTimerEntryHistories.Select(z => z.ChangedBy)),
                                              x => x.EmployeeJobTimers.Select(y => y.EmployeeJobEquipmentTimers),
                                              x => x.Employee,
                                              x => x.Equipment,
                                              x => x.Timesheet.Employee,
                                              x => x.AuthorizeNote,
                                              x => x.Occurrences);
            }

            // if there is pagination then run the query twice, else convert to a list to get the count so that the query only runs once.
            List<EmployeeTimer> result;
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

            return Mapper.Map<IEnumerable<ExpandedEmployeeTimerDto>>(result);
        }

        public ForemanTimesheet GetForemanEmployeeTimers(int employeeID, DateTime day, int departmentID)
        {
            var timesheetID = _timesheetService.FindTimesheetID(employeeID, day.Date, departmentID);
            return timesheetID.HasValue ? _employeeTimerRepository.GetForemanEmployeeTimers(timesheetID.Value) : null;
        }

        public ForemanEmployeeTimer UpdateForemanEmployeeTimer(ForemanEmployeeTimerPatch employeeTimer)
        {
            var employeeTimerID = employeeTimer.ID;
            employeeTimer.EmployeeJobTimers?.ForEach(x => x.EmployeeJobEquipmentTimers = x.EmployeeJobEquipmentTimers?.Where(y => y.EquipmentMinutes.HasValue).ToList());
            employeeTimer.EmployeeJobTimers = employeeTimer.EmployeeJobTimers?.Where(x => x.LaborMinutes.HasValue || (x.EmployeeJobEquipmentTimers?.Any() ?? false)).ToList();

            var result = _employeeTimerRepository.ReplaceEmployeeJobTimerRange(Mapper.Map<EmployeeTimer>(employeeTimer));
            return _employeeTimerRepository.GetForemanEmployeeTimer(result.TimesheetID.Value, employeeTimerID);
        }

        private bool HasValidTimesheetID(EmployeeTimerDto employeeTimer)
        {
            var departmentID = employeeTimer.DepartmentID;
            if (DepartmentGroups.SupervisorMustBeSelf(departmentID))
            {
                var timesheetID = employeeTimer.TimesheetID;
                if (!timesheetID.HasValue)
                {
                    return false;
                }
                var employeeID = employeeTimer.EmployeeID;
                return employeeID == _timesheetService.Find(timesheetID.Value)?.EmployeeID;
            }
            return true;
        }

        #region Occurrences
        public IEnumerable<OccurrenceDto> GetOccurrencesForEmployeeTimer(int employeeTimerID, Pagination pagination, out int resultCount, out int totalResultCount)
        {
            IQueryable<Occurrence> query;
            List<Occurrence> result;
            query = _employeeTimerRepository.GetOccurrencesForEmployeeTimer(employeeTimerID);

            if (pagination != null)
            {
                totalResultCount = query.Count();
                result = query.Pagination<Occurrence>(pagination).ToList<Occurrence>();
                resultCount = result.Count;
            }
            else
            {
                result = query.ToList<Occurrence>();
                resultCount = result.Count;
                totalResultCount = resultCount;
            }
            return Mapper.Map<IEnumerable<OccurrenceDto>>(result);
        }

        public OccurrenceDto AddOccurrence(int employeeTimerID, OccurrenceDto dto)
        {
            return Mapper.Map<OccurrenceDto>(_employeeTimerRepository.AddOccurrence(employeeTimerID, Mapper.Map<Occurrence>(dto)));
        }

        public void RemoveOccurrence(int employeeTimerID, int occurrenceID)
        {
            _employeeTimerRepository.RemoveOccurrence(employeeTimerID, occurrenceID);
        }

        public bool EmployeeTimerOccurrenceExists(int employeeTimerID, int occurrenceID)
        {
            return 0 < _employeeTimerRepository.AllIncluding(x => x.Occurrences).Where(x => x.ID == employeeTimerID && x.Occurrences.Any(y => y.ID == occurrenceID)).Count();
        }

        public IEnumerable<OccurrenceDto> ReplaceOccurrences(int employeeTimerID, IEnumerable<int> occurrences)
        {
            var employeeTimerOccurrences = _employeeTimerRepository.ReplaceOccurrences(employeeTimerID, occurrences);
            return Mapper.Map<IEnumerable<OccurrenceDto>>(employeeTimerOccurrences);
        }

        public int? GetOpenTimerDepartmentID(int employeeID)
        {
            var previousClockinLimit = DateTimeOffset.Now.AddHours(-20);
            return _employeeTimerRepository.AllIncluding(x => x.EmployeeTimerEntries)
                .Where(x => x.EmployeeID == employeeID
                            && x.EmployeeTimerEntries
                                .Any(y => y.ClockIn >= previousClockinLimit && !y.ClockOut.HasValue))
                    .OrderByDescending(x => x.EmployeeTimerEntries.Max(y => y.ClockIn))
                    .FirstOrDefault()?.DepartmentID;
        }

        #endregion Occurrences
        public void SupervisorApproveTimers(int[] employeeTimerIDs)
        {
            if (SupervisorCanApproveTimecards(employeeTimerIDs))
            {
                _employeeTimerRepository.SupervisorApproveTimers(CurrentUser.GetEmployeeID().Value, employeeTimerIDs);
            }
            else
            {
                throw new UnauthorizedDataAccessException(
                    $"Employee {CurrentUser.GetEmployeeID().Value} is not authorized to approve one or more of the timers.");
            }
        }

        public void AccountingApproveTimers(int[] employeeTimerIDs)
        {
            if (CurrentUser.GetRoles().Contains(ProjectConstants.AccountingRole))
            {
                _employeeTimerRepository.AccountingApproveTimers(CurrentUser.GetEmployeeID().Value, employeeTimerIDs);
            }
            else
            {
                throw new UnauthorizedDataAccessException(
                    $"Employee {CurrentUser.GetEmployeeID().Value} is not authorized to approve one or more of the timers.");
            }
        }
        private bool SupervisorCanApproveTimecards(int[] employeeTimerIDs)
        {
            var employeeID = CurrentUser.GetEmployeeID();
            if (employeeID.HasValue)
            {
                return _employeeTimerRepository.CanSupervisorApproveTimers(employeeID.Value, employeeTimerIDs);
            }
            else
            {
                return false;
            }

        }
    }

    public interface IEmployeeTimerService : IGenericService<EmployeeTimerDto>
    {
        #region Occurrences

        IEnumerable<OccurrenceDto> GetOccurrencesForEmployeeTimer(int employeeTimerID, Pagination pagination,
            out int resultCount, out int totalResultCount);

        OccurrenceDto AddOccurrence(int employeeTimerID, OccurrenceDto dto);
        void RemoveOccurrence(int employeeTimerID, int occurrenceID);
        bool EmployeeTimerOccurrenceExists(int employeeTimerID, int occurrenceID);

        #endregion Occurrences

        bool Exists(int employeeID, DateTime day, int departmentID);
        ExpandedEmployeeTimerDto FindExpanded(int employeeID, DateTime day, int departmentID);
        ExpandedEmployeeTimerDto FindExpanded(int employeeID, DateTime day, int departmentID, int supervisorID);

        IEnumerable<ExpandedEmployeeTimerDto> GetAllExpanded(NameValueCollection filter, Pagination pagination,
            out int resultCount, out int totalResultCount);

        IEnumerable<OccurrenceDto> ReplaceOccurrences(int employeeTimerID, IEnumerable<int> occurrences);
        int? GetOpenTimerDepartmentID(int employeeID);
        ForemanEmployeeTimer UpdateForemanEmployeeTimer(ForemanEmployeeTimerPatch employeeTimer);
        ForemanTimesheet GetForemanEmployeeTimers(int employeeID, DateTime day, int departmentID);
        void SupervisorApproveTimers(int[] timecardIDs);
        void AccountingApproveTimers(int[] timecardIDs);
    }
}
