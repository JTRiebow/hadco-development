using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using AutoMapper;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Hosting;
using Hadco.Data;
using Hadco.Services.DataTransferObjects;
using Hadco.Data.Entities;
using Hadco.Common;
using Hadco.Common.Enums;
using Hadco.Common.Exceptions;

namespace Hadco.Services
{
    public class EmployeeTimerEntryService : GenericService<EmployeeTimerEntryDto, EmployeeTimerEntry>, IEmployeeTimerEntryService
    {
        private IEmployeeRepository _employeeRepository;
        private IEmployeeTimerEntryRepository _employeeTimerEntryRepository;
        private IEmployeeTimerRepository _employeeTimerRepository;
        private IEmployeeTimerEntryHistoryRepository _employeeTimerEntryHistoryRepository;
        private IJobTimerRepository _jobTimerRepository;
        private IDowntimeTimerRepository _downtimeTimerRepository;
        private INoteRepository _noteRepository;
        private IPermissionsService _permissionsService;

        public EmployeeTimerEntryService(IEmployeeTimerEntryRepository employeeTimerEntryRepository,
                                            IPrincipal currentUser,
                                            IEmployeeRepository employeeRepository,
                                            IEmployeeTimerRepository employeeTimerRepository,
                                            IEmployeeTimerEntryHistoryRepository employeeTimerEntryHistoryRepository,
                                            IJobTimerRepository jobTimerRepository,
                                            IDowntimeTimerRepository downtimeTimerRepository,
                                            INoteRepository noteRepository,
                                            IPermissionsService permissionsService)
            : base(employeeTimerEntryRepository, currentUser)
        {
            _employeeRepository = employeeRepository;
            _employeeTimerEntryRepository = employeeTimerEntryRepository;
            _employeeTimerRepository = employeeTimerRepository;
            _employeeTimerEntryHistoryRepository = employeeTimerEntryHistoryRepository;
            _jobTimerRepository = jobTimerRepository;
            _downtimeTimerRepository = downtimeTimerRepository;
            _noteRepository = noteRepository;
            _permissionsService = permissionsService;
        }

        public override void Delete(int id)
        {
            var employeeTimer = _employeeTimerEntryRepository.Find(id, x => x.EmployeeTimer)?.EmployeeTimer;
            base.Delete(id);
            if (employeeTimer != null)
            {
                AllocateDowntime(employeeTimer);
            }
        }

        public override EmployeeTimerEntryDto Insert(EmployeeTimerEntryDto dto)
        {
            var currentEmployeeID = CurrentUser.GetEmployeeID();
            var employeeTimer = _employeeTimerRepository.Find(dto.EmployeeTimerID);
            if (!EmployeeCanEditTimer(employeeTimer.EmployeeID) &&
                !_permissionsService.HasPermission(AuthActivityID.AddEmployeeTimerEntry, employeeTimer.DepartmentID))
            {
                throw new UnauthorizedDataAccessException($"Employee {currentEmployeeID} is not authorized to edit the timer.");
            }


            FixDateMismatches(ref dto, employeeTimer);

            var employeeTimerEntry = Mapper.Map<EmployeeTimerEntry>(dto);
            if (!string.IsNullOrWhiteSpace(dto.ClockInNote))
            {
                var clockInNote = new Note()
                {
                    EmployeeID = employeeTimer.EmployeeID,
                    Day = employeeTimer.Day,
                    DepartmentID = employeeTimer.DepartmentID,
                    Description = dto.ClockInNote,
                    NoteTypeID = (int)NoteTypeName.ClockIn,
                    CreatedTime = DateTimeOffset.Now,
                    CreatedEmployeeID = currentEmployeeID
                };
                employeeTimerEntry.ClockInNote = clockInNote;
            }
            if (!string.IsNullOrWhiteSpace(dto.ClockOutNote))
            {
                var clockOutNote = new Note()
                {
                    EmployeeID = employeeTimer.EmployeeID,
                    Day = employeeTimer.Day,
                    DepartmentID = employeeTimer.DepartmentID,
                    Description = dto.ClockOutNote,
                    NoteTypeID = (int)NoteTypeName.ClockOut,
                    CreatedTime = DateTimeOffset.Now,
                    CreatedEmployeeID = currentEmployeeID
                };
                employeeTimerEntry.ClockOutNote = clockOutNote;
            }

            if (OverlapsWithExistingEmployeeTimerEntry(dto))
            {
                throw new TimerOverlapException("This timer entry conflicts with an exististing timer entry on this Employee's timer");
            }

            var employeeTimerEntryDto = Mapper.Map<EmployeeTimerEntryDto>(_employeeTimerEntryRepository.Insert(employeeTimerEntry));
            HostingEnvironment.QueueBackgroundWorkItem(ct => PostInsert(currentEmployeeID, employeeTimer, employeeTimerEntryDto));
            return employeeTimerEntryDto;
        }

        public override EmployeeTimerEntryDto Update(EmployeeTimerEntryDto dto)
        {
            var employeeTimerEntry = _employeeTimerEntryRepository.Find(dto.ID);
            var employeeTimer = _employeeTimerRepository.Find(dto.EmployeeTimerID);

            var currentEmployeeID = CurrentUser.GetEmployeeID().Value;
            if (!EmployeeCanEditTimer(employeeTimer.EmployeeID) &&
                !_permissionsService.HasPermission(AuthActivityID.EditEmployeeTimerEntry, employeeTimer.DepartmentID))
            {
                throw new UnauthorizedAccessException($"Employee {currentEmployeeID} is not authorized to edit the timer.");
            }

            FixDateMismatches(ref dto, employeeTimer);

            AddEmployeeTimerEntryHistory(dto, employeeTimerEntry, currentEmployeeID);

            var hasClockInNote = dto.ClockInNote != null
                && employeeTimerEntry.ClockInNote != null;
            if (hasClockInNote)
            {
                if (dto.ClockInNote != employeeTimerEntry.ClockInNote.Description)
                {
                    var clockInNoteID = employeeTimerEntry.ClockInNote.ID;
                    employeeTimerEntry.ClockInNoteID = null;
                    _employeeTimerEntryRepository.Update(employeeTimerEntry);
                    _noteRepository.Delete(clockInNoteID);
                }
                else
                {
                    dto.ClockInNote = null;
                }
            }

            var hasClockOutNote = dto.ClockOutNote != null
                && employeeTimerEntry.ClockOutNote != null;
            if (hasClockOutNote)
            {
                var clockOutNoteID = employeeTimerEntry.ClockOutNote.ID;
                if (dto.ClockOutNote != employeeTimerEntry.ClockOutNote.Description)
                {
                    employeeTimerEntry.ClockOutNoteID = null;
                    _employeeTimerEntryRepository.Update(employeeTimerEntry);
                    _noteRepository.Delete(clockOutNoteID);
                }
                else
                {
                    dto.ClockOutNote = null;
                }
            }

            Mapper.Map(dto, employeeTimerEntry);

            if (!string.IsNullOrWhiteSpace(dto.ClockInNote))
            {
                var clockInNote = new Note()
                {
                    EmployeeID = employeeTimer.EmployeeID,
                    Day = employeeTimer.Day,
                    DepartmentID = employeeTimer.DepartmentID,
                    Description = dto.ClockInNote,
                    NoteTypeID = (int)NoteTypeName.ClockIn,
                    CreatedTime = DateTimeOffset.Now,
                    CreatedEmployeeID = currentEmployeeID
                };
                employeeTimerEntry.ClockInNote = clockInNote;
            }
            if (!string.IsNullOrWhiteSpace(dto.ClockOutNote))
            {
                var clockOutNote = new Note()
                {
                    EmployeeID = employeeTimer.EmployeeID,
                    Day = employeeTimer.Day,
                    DepartmentID = employeeTimer.DepartmentID,
                    Description = dto.ClockOutNote,
                    NoteTypeID = (int)NoteTypeName.ClockOut,
                    CreatedTime = DateTimeOffset.Now,
                    CreatedEmployeeID = currentEmployeeID
                };
                employeeTimerEntry.ClockOutNote = clockOutNote;
            }
            _employeeTimerEntryRepository.Save();

            BackgroundTaskScheduler.QueueBackgroundWorkItem(ct => PostUpdate(employeeTimer, currentEmployeeID));

            return Mapper.Map<EmployeeTimerEntryDto>(employeeTimerEntry);
        }

        private void AddEmployeeTimerEntryHistory(EmployeeTimerEntryDto newEntry, EmployeeTimerEntry oldEntry, int currentEmployeeID)
        {
            var hasChanged = oldEntry.ClockIn != newEntry.ClockIn || oldEntry.ClockOut != newEntry.ClockOut;
            if (hasChanged)
            {
                var eteh = new EmployeeTimerEntryHistory()
                {
                    EmployeeTimerEntryID = newEntry.ID,
                    PreviousClockIn = oldEntry.ClockIn,
                    PreviousClockOut = oldEntry.ClockOut,
                    CurrentClockIn = newEntry.ClockIn,
                    CurrentClockOut = newEntry.ClockOut,
                    ChangedByID = currentEmployeeID,
                    ChangedTime = DateTimeOffset.Now
                };
                _employeeTimerEntryHistoryRepository.Insert(eteh);
            }
        }

        private void PostUpdate(EmployeeTimer employeeTimer, int currentEmployeeID)
        {
            AllocateDowntime(employeeTimer);
            _noteRepository.TriggerFlag(NoteTypeName.TooManyHours, employeeTimer.EmployeeID, employeeTimer.Day, employeeTimer.DepartmentID, currentEmployeeID);
        }

        private void PostInsert(int? currentEmployeeID, EmployeeTimer employeeTimer, EmployeeTimerEntryDto employeeTimerEntryDto)
        {
            var userIsNotEmployee = currentEmployeeID.HasValue && currentEmployeeID.Value != employeeTimer.EmployeeID;
            if (userIsNotEmployee)
            {
                var eteh = new EmployeeTimerEntryHistory()
                {
                    EmployeeTimerEntryID = employeeTimerEntryDto.ID,
                    PreviousClockIn = null,
                    PreviousClockOut = null,
                    CurrentClockIn = employeeTimerEntryDto.ClockIn,
                    CurrentClockOut = employeeTimerEntryDto.ClockOut,
                    ChangedByID = currentEmployeeID.Value,
                    ChangedTime = DateTimeOffset.Now
                };
                _employeeTimerEntryHistoryRepository.Insert(eteh);
            }

            AllocateDowntime(employeeTimer);
            _noteRepository.TriggerFlag(NoteTypeName.TooManyHours, employeeTimer.EmployeeID, employeeTimer.Day, employeeTimer.DepartmentID, CurrentUser.GetEmployeeID().Value);
        }

        private static void FixDateMismatches(ref EmployeeTimerEntryDto dto, EmployeeTimer employeeTimer)
        {
            var day = employeeTimer.Day;
            var clockInDiff = (int)(day - dto.ClockIn).TotalHours;
            if (Math.Abs(clockInDiff) > 36)
            {
                dto.ClockIn = dto.ClockIn.AddDays(clockInDiff / 24 + 1);
            }
            var clockOutDiff = (int)((day - dto.ClockOut)?.TotalHours ?? 0);
            if (Math.Abs(clockOutDiff) > 36)
            {
                dto.ClockOut = dto.ClockOut?.AddDays(clockOutDiff / 24 + 1);
            }
            if (dto.ClockOut.HasValue && dto.ClockOut < dto.ClockIn)
            {
                throw new HttpRequestException("Clock-in cannot be later than clock-out");
            }
        }

        private bool OverlapsWithExistingEmployeeTimerEntry(EmployeeTimerEntryDto dto)
        {
            return _employeeTimerEntryRepository.All?.Where(x => x.EmployeeTimerID == dto.EmployeeTimerID
                                                    && x.ID != dto.ID
                                                    && ((x.ClockIn < dto.ClockIn && dto.ClockIn < x.ClockOut)
                                                    || (x.ClockIn < dto.ClockOut && dto.ClockOut < x.ClockOut)
                                                    || x.ClockOut == null)).Any() ?? false;
        }

        // TODO This method needs to be refactored to use the permissions system once we start using it fully on the backend.
        // The current implementation is focused on not breaking what has already been tested, while allowing foremen on mobile
        // edit timers.
        private bool EmployeeCanEditTimer(int timerEmployeeID)
        {
            var currentEmployeeID = CurrentUser.GetEmployeeID();
            if (currentEmployeeID == null)
            {
                return false;
            }
            if (currentEmployeeID == timerEmployeeID)
            {
                return true;
            }
            var roles = CurrentUser.GetRoles();
            var rolesThatCanEditAnyTimer = new[] {
                ProjectConstants.AdminRole,
                ProjectConstants.AccountingRole,
                ProjectConstants.SupervisorRole,
                ProjectConstants.ManagerRole,
                ProjectConstants.BillingRole,
                ProjectConstants.SuperintendentRole,
                ProjectConstants.ForemanRole};
            if (roles.Intersect(rolesThatCanEditAnyTimer).Any())
            {
                return true;
            }

            return _employeeRepository.GetDirectSupervisorsForEmployee(timerEmployeeID)
                .Any(x => x.ID == currentEmployeeID.Value);
        }

        private void AllocateDowntime(EmployeeTimer employeeTimer)
        {
            var departmentID = employeeTimer.DepartmentID;
            var timesheetID = employeeTimer.TimesheetID;
            if (departmentID == (int)DepartmentName.Trucking || departmentID == (int)DepartmentName.Transport && timesheetID.HasValue)
            {
                _downtimeTimerRepository.AddLoadDowntimeTimers(timesheetID.Value);
            }
            if (departmentID == (int)DepartmentName.Mechanic && timesheetID.HasValue)
            {
                _jobTimerRepository.AddMechanicDowntime(timesheetID.Value);
            }
        }
    }

    public interface IEmployeeTimerEntryService : IGenericService<EmployeeTimerEntryDto>
    {

    }
}

