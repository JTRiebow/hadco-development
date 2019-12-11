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

namespace Hadco.Services
{
    public class EmployeeJobEquipmentTimerService : GenericService<EmployeeJobEquipmentTimerDto, EmployeeJobEquipmentTimer>, IEmployeeJobEquipmentTimerService
    {
        private IEmployeeJobEquipmentTimerRepository _employeeJobEquipmentTimerRepository;
        private IEmployeeJobTimerRepository _employeeJobTimerRepository;
        public EmployeeJobEquipmentTimerService(IEmployeeJobEquipmentTimerRepository employeeJobEquipmentTimerRepository,
            IEmployeeJobTimerRepository employeeJobTimerRepository,
            IPrincipal currentUser)
            : base(employeeJobEquipmentTimerRepository, currentUser)
        {
            _employeeJobEquipmentTimerRepository = employeeJobEquipmentTimerRepository;
            _employeeJobTimerRepository = employeeJobTimerRepository;
        }

        public override EmployeeJobEquipmentTimerDto Insert(EmployeeJobEquipmentTimerDto dto)
        {
            if (!dto.EmployeeJobTimerID.HasValue && dto.EmployeeTimerID.HasValue && dto.JobTimerID.HasValue)
            {
                var employeeJobTimerID =
                    _employeeJobTimerRepository.All
                        .FirstOrDefault(x => x.EmployeeTimerID == dto.EmployeeTimerID && x.JobTimerID == dto.JobTimerID)?.ID;
                if (!employeeJobTimerID.HasValue)
                {
                    employeeJobTimerID = _employeeJobTimerRepository.Insert(new EmployeeJobTimer()
                    {
                        EmployeeTimerID = dto.EmployeeTimerID.Value,
                        JobTimerID = dto.JobTimerID.Value,
                        LaborMinutes = 0
                    }).ID;
                }
                dto.EmployeeJobTimerID = employeeJobTimerID;
            }
            return base.Insert(dto);
        }
    }

    public interface IEmployeeJobEquipmentTimerService : IGenericService<EmployeeJobEquipmentTimerDto>
    {

    }
}
