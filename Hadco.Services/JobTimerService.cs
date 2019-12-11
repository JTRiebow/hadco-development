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
using System.Collections.Specialized;
using System.Data.Entity.Core;
using Hadco.Common;
using System.IO;
using CsvHelper;
using Hadco.Common.DataTransferObjects;
using Hadco.Common.Enums;

namespace Hadco.Services
{
    public class JobTimerService : GenericService<JobTimerDto, JobTimer>, IJobTimerService
    {
        private IJobTimerRepository _jobTimerRepository;
        private ITimesheetRepository _timesheetRepository;
        private IEmployeeTimerRepository _employeeTimerRepository;
        public JobTimerService(IJobTimerRepository jobTimerRepository, ITimesheetRepository timesheetRepository, IEmployeeTimerRepository employeeTimerRepository, IPrincipal currentUser)
            : base(jobTimerRepository, currentUser)
        {
            _jobTimerRepository = jobTimerRepository;
            _timesheetRepository = timesheetRepository;
            _employeeTimerRepository = employeeTimerRepository;
        }

        public override JobTimerDto Insert(JobTimerDto dto)
        {
            CheckForTimerOverlap(dto);
            var jobTimer = base.Insert(dto);
            _jobTimerRepository.AddMechanicDowntime(jobTimer.TimesheetID);
            return jobTimer;
        }

        public override JobTimerDto Update(JobTimerDto dto)
        {
            var jobTimer = base.Update(dto);
            _jobTimerRepository.AddMechanicDowntime(jobTimer.TimesheetID);
            return jobTimer;
        }

        public override void Delete(int id)
        {
            var timesheetID = _jobTimerRepository.Find(id).TimesheetID;
            base.Delete(id);
            _jobTimerRepository.AddMechanicDowntime(timesheetID);
        }

        public ExpandedJobTimerFromJobTimerDto FindExpanded(int jobTimerID)
        {
            var jobTimer = _jobTimerRepository.AllIncluding(y => y.Job,
                                                    y => y.Phase,
                                                    y => y.Category,
                                                    y => y.EmployeeJobTimers.Select(z => z.EmployeeTimer.Employee),
                                                    y => y.EmployeeJobTimers.Select(z => z.EmployeeJobEquipmentTimers.Select(a => a.Equipment))).SingleOrDefault(x => x.ID == jobTimerID);
            if (jobTimer == null)
            {
                return null;
            }
            var expandedTimer = Mapper.Map<ExpandedJobTimerFromJobTimerDto>(jobTimer);
            expandedTimer.PreviousQuantity = _jobTimerRepository.GetQuantities(jobTimerID).PreviousQuantity;
            expandedTimer.OtherNewQuantity = _jobTimerRepository.GetQuantities(jobTimerID).OtherNewQuantity;          
            
            return expandedTimer;
        }

        public IEnumerable<ExpandedJobTimerFromJobTimerDto> GetAllExpanded(int timesheetID, NameValueCollection filter, Pagination pagination, out int resultCount, out int totalResultCount)
        {
            IQueryable<JobTimer> query;
            if (filter != null)
            {
                query = _jobTimerRepository.AllIncluding(y => y.Job,
                                              y => y.Phase,
                                              y => y.Category,
                                              y => y.EmployeeJobTimers.Select(z => z.EmployeeTimer.Employee),
                                              y => y.EmployeeJobTimers.Select(z => z.EmployeeJobEquipmentTimers.Select(a => a.Equipment))).Filter(filter);
            }
            else
            {
                query = _jobTimerRepository.AllIncluding(y => y.Job,
                                              y => y.Phase,
                                              y => y.Category,
                                              y => y.EmployeeJobTimers.Select(z => z.EmployeeTimer.Employee),
                                              y => y.EmployeeJobTimers.Select(z => z.EmployeeJobEquipmentTimers.Select(a => a.Equipment)));
            }

            query = query.Where(x => x.TimesheetID == timesheetID);

            // if there is pagination then run the query twice, else convert to a list to get the count so that the query only runs once.
            List<JobTimer> result;
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

            return Mapper.Map<IEnumerable<ExpandedJobTimerFromJobTimerDto>>(result);
        }

        public IEnumerable<ExpandedJobTimerFromJobTimerDto> GetJobTimersForEmployee(int employeeID, DateTime day, int departmentID)
        {
            var timesheetIDs = _employeeTimerRepository.All
                                    .Where(x => x.EmployeeID == employeeID && x.Day == day && x.DepartmentID == departmentID)
                                    .Select(x=>x.TimesheetID).ToList();

            IEnumerable<JobTimer> jobTimers = new List<JobTimer>();
            if (timesheetIDs.Any())
            {
                 jobTimers = _jobTimerRepository.AllIncluding(y => y.Job,
                                                    y => y.Phase,
                                                    y => y.Category,
                                                    y => y.Timesheet.Employee).Where(x => timesheetIDs.Contains(x.TimesheetID)).ToList();
            }
            return Mapper.Map<IEnumerable<ExpandedJobTimerFromJobTimerDto>>(jobTimers);
        }

        private void CheckForTimerOverlap(JobTimerDto dto)
        {
            var timesheet = _timesheetRepository.Find(dto.TimesheetID);
            if (timesheet == null)
            {
                throw new ObjectNotFoundException($"Timesheet with ID {dto.TimesheetID} not found");
            }
            var employeeID = timesheet.EmployeeID;
            var overlappingTimers = _jobTimerRepository.AllIncluding(x => x.Timesheet, x => x.Category)
                                            .Where(x => x.Timesheet.EmployeeID == employeeID)
                                            .Where(x => x.ID != dto.ID)
                                            .Where(x => x.Category.CategoryNumber != "Down")
                                            .Where(x => (x.StartTime > dto.StartTime &&
                                                         x.StartTime < dto.StopTime) ||
                                                        (x.StopTime > dto.StartTime &&
                                                         x.StopTime < dto.StopTime));
            if (overlappingTimers.Any())
            {
                throw new Exception("Cannot update job timer due to overlap with an existing job timer for this employee");
            }
            // TODO Add OverlappingTimesCheck as SQL function - Determine when timechecking is needed
        }
    }

    public interface IJobTimerService : IGenericService<JobTimerDto>
    {
        ExpandedJobTimerFromJobTimerDto FindExpanded(int jobTimerID);
        IEnumerable<ExpandedJobTimerFromJobTimerDto> GetAllExpanded(int timesheetID, NameValueCollection filter, Pagination pagination, out int resultCount, out int totalResultCount);
        IEnumerable<ExpandedJobTimerFromJobTimerDto> GetJobTimersForEmployee(int employeeID, DateTime day, int departmentID);
    }
}
