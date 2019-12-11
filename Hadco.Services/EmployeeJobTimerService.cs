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
using Hadco.Common.DataTransferObjects;
using System.Collections.Specialized;
using Hadco.Common;

namespace Hadco.Services
{
    public class EmployeeJobTimerService : GenericService<EmployeeJobTimerDto, EmployeeJobTimer>, IEmployeeJobTimerService
    {
        private IEmployeeJobTimerRepository _employeeJobTimerRepository;
        public EmployeeJobTimerService(IEmployeeJobTimerRepository employeeJobTimerRepository, IPrincipal currentUser)
            : base(employeeJobTimerRepository, currentUser)
        {
            _employeeJobTimerRepository = employeeJobTimerRepository;
        }

        public IEnumerable<EmployeeJobTimerSummaryDto> GetEmployeeSummary(int jobTimerID)
        {
            return _employeeJobTimerRepository.GetEmployeeSummary(jobTimerID);
        }

        public IEnumerable<EmployeeJobTimerPrimaryDto> GetByEmployeeExpanded(int employeeID, DateTime day, int departmentID, NameValueCollection filter, Pagination pagination, out int resultCount, out int totalResultCount)
        {
            IQueryable<EmployeeJobTimer> query;
            if (filter != null)
            {
                query = _employeeJobTimerRepository.AllIncluding(
                                              x => x.JobTimer.Job,
                                              x => x.JobTimer.Phase,
                                              x => x.JobTimer.Category,
                                              x => x.EmployeeTimer,
                                              x => x.EmployeeTimer.Timesheet.Employee,
                                              x => x.EmployeeJobEquipmentTimers.Select(y => y.Equipment)).Filter(filter);
            }
            else
            {
                query = _employeeJobTimerRepository.AllIncluding(
                                              x => x.JobTimer.Job,
                                              x => x.JobTimer.Phase,
                                              x => x.JobTimer.Category,
                                              x => x.EmployeeTimer,
                                              x => x.EmployeeTimer.Timesheet.Employee,
                                              x => x.EmployeeJobEquipmentTimers.Select(y => y.Equipment));
            }

            query = query.Where(x => x.EmployeeTimer.EmployeeID == employeeID && x.EmployeeTimer.DepartmentID == departmentID && x.EmployeeTimer.Day == day.Date);

            // if there is pagination then run the query twice, else convert to a list to get the count so that the query only runs once.
            List<EmployeeJobTimer> result;
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

            return Mapper.Map<IEnumerable<EmployeeJobTimerPrimaryDto>>(result);
        }

        public IEnumerable<EmployeeJobTimerExpandedDto> GetAllExpanded(NameValueCollection filter, Pagination pagination, out int resultCount, out int totalResultCount)
        {
            IQueryable<EmployeeJobTimer> query;
            if (filter != null)
            {
                query = _employeeJobTimerRepository.AllIncluding(
                                              x => x.EmployeeTimer,
                                              x => x.EmployeeJobEquipmentTimers.Select(y=>y.Equipment)).Filter(filter);
            }
            else
            {
                query = _employeeJobTimerRepository.AllIncluding(
                                              x => x.EmployeeTimer,
                                              x => x.EmployeeJobEquipmentTimers.Select(y => y.Equipment));
            }


            // if there is pagination then run the query twice, else convert to a list to get the count so that the query only runs once.
            List<EmployeeJobTimer> result;
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

            return Mapper.Map<IEnumerable<EmployeeJobTimerExpandedDto>>(result);
        }
    }

    public interface IEmployeeJobTimerService : IGenericService<EmployeeJobTimerDto>
    {
        IEnumerable<EmployeeJobTimerPrimaryDto> GetByEmployeeExpanded(int employeeID, DateTime day, int departmentID, NameValueCollection filter, Pagination pagination, out int resultCount, out int totalResultCount);
        IEnumerable<EmployeeJobTimerSummaryDto> GetEmployeeSummary(int jobTimerID);
        IEnumerable<EmployeeJobTimerExpandedDto> GetAllExpanded(NameValueCollection filter, Pagination pagination, out int resultCount, out int totalResultCount);
    }
}
