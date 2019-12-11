using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using System.Security.Principal;
using CsvHelper;
using Hadco.Common.DataTransferObjects;
using Hadco.Common.Enums;
using Hadco.Data;
using Hadco.Services.DataTransferObjects;
using Hadco.Data.Entities;

namespace Hadco.Services
{
    public class TimesheetService : GenericService<TimesheetDto, Timesheet>, ITimesheetService
    {
        private ITimesheetRepository _timesheetRepository;
        private IDepartmentService _departmentService;
        public TimesheetService(ITimesheetRepository timesheetRepository, IDepartmentService departmentService, IPrincipal currentUser)
            : base(timesheetRepository, currentUser)
        {
            _timesheetRepository = timesheetRepository;
            _departmentService = departmentService;
        }

        public int? FindTimesheetID(int employeeID, DateTime day, int departmentID)
        {
            return _timesheetRepository.FindTimesheetID(employeeID, day.Date, departmentID);
        }

        public int? GetOdometer(int employeeID, DateTime day, int departmentID)
        {
            return _timesheetRepository.GetOdometer(employeeID, day, departmentID);
        }

        public bool Exists(int employeeID, DateTime day, int departmentID)
        {
            var exists = _timesheetRepository.FindTimesheetID(employeeID, day.Date, departmentID).HasValue;
            return exists;
        }

        public BaseTimesheetDto FindPatch(int id)
        {
            return Mapper.Map<BaseTimesheetDto>(_timesheetRepository.FindNoTracking(id));
        }

        public TimesheetDto Update(BaseTimesheetDto dto)
        {
            var entity = _timesheetRepository.Find(dto.ID);
            Mapper.Map(dto, entity);
            _timesheetRepository.Save();
            return Mapper.Map<TimesheetDto>(entity);
        }

        public ExpandedTimesheetDto FindExpanded(int employeeID, DateTime day, int departmentID, bool isMobile)
        {
            var includeExpressions = GetIncludeExpressions(departmentID);

            var timesheet = _timesheetRepository
            .AllIncluding(includeExpressions.ToArray())
                          .SingleOrDefault(x => x.EmployeeID == employeeID && x.Day == day.Date && x.DepartmentID == departmentID);
         
            var expandedTimesheet = Mapper.Map<ExpandedTimesheetDto>(timesheet);

            if (expandedTimesheet != null && !isMobile)
                expandedTimesheet.DowntimeTimers = expandedTimesheet.DowntimeTimers.Where(x => !x.LoadTimerID.HasValue);

            if (expandedTimesheet?.JobTimers != null)
            {
                foreach (var jobTimer in expandedTimesheet.JobTimers)
                {
                    jobTimer.Equipment = expandedTimesheet.EmployeeTimers
                        .SelectMany(
                            x =>
                                x.EmployeeJobTimers.Where(y => y.JobTimerID == jobTimer.ID)
                                    .SelectMany(y => y.EmployeeJobEquipmentTimers.Select(z => z.Equipment))).ToList();
                }
            }

            return expandedTimesheet;
        }

        private static List<Expression<Func<Timesheet, object>>> GetIncludeExpressions(int departmentID)
        {
            var includeExpressions = new List<Expression<Func<Timesheet, object>>>()
                                     {
                                         x => x.EmployeeTimers.Select(y => y.Employee),
                                         x => x.EmployeeTimers.Select(y => y.Occurrences),
                                         x =>
                                             x.EmployeeTimers
                                              .Select(y => y.EmployeeTimerEntries.Select(z => z.EmployeeTimerEntryHistories)),
                                         x => x.EmployeeTimers.Select(y => y.EmployeeTimecard),
                                         x => x.EmployeeTimers.Select(y => y.Equipment), // Which Departments use this
                                     };

            if (departmentID == (int) DepartmentName.Trucking || departmentID == (int) DepartmentName.Transport)
            {
                includeExpressions.AddRange(new Expression<Func<Timesheet, object>>[]
                                            {
                                                x => x.DowntimeTimers.Select(z => z.DowntimeReason),
                                                x => x.LoadTimers.Select(y => y.Job),
                                                x => x.LoadTimers.Select(y => y.Phase),
                                                x => x.LoadTimers.Select(y => y.Category),
                                                x => x.LoadTimers.Select(y => y.Truck),
                                                x => x.LoadTimers.Select(y => y.Trailer),
                                                x => x.LoadTimers.Select(y => y.Pup),
                                                x => x.LoadTimers.Select(y => y.BillType),
                                                x => x.LoadTimers.Select(y => y.Material),
                                                x => x.LoadTimers.Select(y => y.LoadTimerEntries),
                                                x => x.LoadTimers.Select(y => y.DowntimeTimers
                                                                                 .Select(z => z.DowntimeReason))
                                            });
                if (departmentID == (int) DepartmentName.Transport)
                {
                    includeExpressions.AddRange(new Expression<Func<Timesheet, object>>[]
                                                {
                                                    x => x.LoadTimers.Select(y => y.LoadEquipment)
                                                });
                }
            }
            else
            {
                includeExpressions.AddRange(new Expression<Func<Timesheet, object>>[]
                                            {
                                                x => x.Equipment,
                                                x =>
                                                    x.EquipmentTimers,
                                                x => x.EquipmentTimers.Select(y => y.EquipmentTimerEntries),
                                                x => x.JobTimers.Select(y => y.Job),
                                                x => x.JobTimers.Select(y => y.Phase),
                                                x => x.JobTimers.Select(y => y.Category),
                                                x =>
                                                    x
                                                        .JobTimers.Select(y => y.EmployeeJobTimers
                                                                                .Select(z => z
                                                                                            .EmployeeJobEquipmentTimers
                                                                                            .Select(a => a.Equipment))),
                                            });
            }
            return includeExpressions;
        }


        public IEnumerable<SuperintendentTimesheetsDto> GetForemanTimesheets(int superintendentID, DateTime week)
        {
            return _timesheetRepository.GetForemenTimesheetsForSupervisor(superintendentID, week);
        }

        public IEnumerable<SuperintendentTimesheetsDto> GetForemanTimesheetsFromEmployee(int employeeID, DateTime week)
        {
            return _timesheetRepository.GetForemanTimesheetsFromEmployee(employeeID, week);
        }

        public TimesheetDto GetOrCreateTimesheet(int employeeID, DateTime day, int departmentID)
        {
            var timesheet = _timesheetRepository.All.FirstOrDefault(x => x.EmployeeID == employeeID && x.DepartmentID == departmentID && x.Day == day);
            if (timesheet == null)
            {
                var ts = new Timesheet()
                {
                    EmployeeID = employeeID,
                    DepartmentID = departmentID,
                    Day = day
                };
                timesheet = _timesheetRepository.Insert(ts);
            }
            return Mapper.Map<TimesheetDto>(timesheet);
        }
    }
    public interface ITimesheetService : IGenericService<TimesheetDto>
    {
        bool Exists(int employeeID, DateTime day, int departmentID);
        TimesheetDto Update(BaseTimesheetDto dto);
        BaseTimesheetDto FindPatch(int id);
        ExpandedTimesheetDto FindExpanded(int employeeID, DateTime day, int departmentID, bool isMobile);
        IEnumerable<SuperintendentTimesheetsDto> GetForemanTimesheets(int superintendentID, DateTime week);
        IEnumerable<SuperintendentTimesheetsDto> GetForemanTimesheetsFromEmployee(int employeeID, DateTime week);
        TimesheetDto GetOrCreateTimesheet(int employeeID, DateTime day, int departmentID);
        int? FindTimesheetID(int employeeID, DateTime day, int departmentID);
        int? GetOdometer(int employeeID, DateTime day, int departmentID);
    }
}

