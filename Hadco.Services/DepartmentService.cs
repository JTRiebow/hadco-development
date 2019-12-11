using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using System.Security.Principal;
using Hadco.Common;
using Hadco.Common.Enums;
using Hadco.Data;
using Hadco.Services.DataTransferObjects;
using Hadco.Data.Entities;
using System.Web.Caching;

namespace Hadco.Services
{
    public class DepartmentService : GenericService<DepartmentDto, Department>, IDepartmentService
    {
        private IDepartmentRepository _departmentRepository;
        private IEmployeeService _employeeService;
        public DepartmentService(IDepartmentRepository departmentRepository, IEmployeeService employeeService, IPrincipal currentUser)
            : base(departmentRepository, currentUser)
        {
            _departmentRepository = departmentRepository;
            _employeeService = employeeService;
        }

        public IEnumerable<DepartmentDto> GetFromCache()
        {
            var departmentsDtos = MemoryCache.Default.Get(typeof(DepartmentDto).Name) as IEnumerable<DepartmentDto>;
            if (departmentsDtos == null)
            {
                departmentsDtos = Mapper.Map<IEnumerable<DepartmentDto>>(_departmentRepository.All);
                MemoryCache.Default.Set(typeof(DepartmentDto).Name, departmentsDtos, DateTimeOffset.MaxValue);
            }
            return departmentsDtos;
        }

        public int GetSubDepartment(int employeeID, int departmentID)
        {
            if (departmentID == (int)DepartmentName.Concrete)
            {
                var isConcrete2HEmployee = _employeeService.EmployeeDepartmentExists(employeeID, (int)DepartmentName.Concrete2H);
                var isConcreteHBEmployee = _employeeService.EmployeeDepartmentExists(employeeID, (int)DepartmentName.ConcreteHB);
                if (isConcrete2HEmployee && !isConcreteHBEmployee)
                {
                    return (int)DepartmentName.Concrete2H;
                }
                if (isConcreteHBEmployee && !isConcrete2HEmployee)
                {
                    return (int)DepartmentName.ConcreteHB;
                }
            }
            return departmentID;
        }
        public PatchDepartmentDto FindPatch(int id)
        {
            return Mapper.Map<PatchDepartmentDto>(_departmentRepository.FindNoTracking(id));
        }
    }

    public interface IDepartmentService : IGenericService<DepartmentDto>
    {
        IEnumerable<DepartmentDto> GetFromCache();

        int GetSubDepartment(int employeeID, int departmentID);

        PatchDepartmentDto FindPatch(int id);
    }
}
