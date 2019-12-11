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

namespace Hadco.Services
{
	public class EmployeeTypeService : GenericService<EmployeeTypeDto, EmployeeType>, IEmployeeTypeService
    {
	    private IEmployeeTypeRepository _employeeTypeRepository;
        public EmployeeTypeService(IEmployeeTypeRepository employeeTypeRepository, IPrincipal currentUser)
            : base(employeeTypeRepository, currentUser)
		{
			_employeeTypeRepository = employeeTypeRepository;
		}
	}

	public interface IEmployeeTypeService : IGenericService<EmployeeTypeDto>
	{

	}
}
