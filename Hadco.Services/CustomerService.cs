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
	public class CustomerService : GenericService<CustomerDto, Customer>, ICustomerService
    {
	    private ICustomerRepository _customerRepository;
        public CustomerService(ICustomerRepository customerRepository, IPrincipal currentUser)
            : base(customerRepository, currentUser)
		{
			_customerRepository = customerRepository;
		}
        
	}

	public interface ICustomerService : IGenericService<CustomerDto>
	{

	}
}
