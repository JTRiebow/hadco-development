using AutoMapper;
using Hadco.Common;
using Hadco.Data;
using Hadco.Services.DataTransferObjects;
using System;
using System.Data.Entity;
using System.Linq;

namespace Hadco.Services
{
    public class EmployeeAuthenticationService : IEmployeeAuthenticationService
    {
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeeAuthenticationService(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public ExpandedEmployeeDto Find(string username)
        {
            var result = _employeeRepository.Find(username);
            return Mapper.Map<ExpandedEmployeeDto>(result);
        }

        public bool Authenticate(string username, string password)
        {
            var employee = _employeeRepository.All.SingleOrDefault(x => x.Username == username);

            bool isLegitimateLogin = employee != null && employee.Status == EntityStatus.Active && PasswordHash.ValidatePassword(password, employee.Password);

            if (isLegitimateLogin)
            {
                employee.LastAuthenticatedOn = DateTimeOffset.Now;
                _employeeRepository.Save();
                return true;
            }
            return false;
        }
    }

    public interface IEmployeeAuthenticationService
    {
        bool Authenticate(string username, string password);

        ExpandedEmployeeDto Find(string username);
    }
}