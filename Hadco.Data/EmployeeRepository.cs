using Hadco.Data.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Hadco.Common;
using Hadco.Common.DataTransferObjects;

namespace Hadco.Data
{
    public class EmployeeRepository : GenericRepository<Employee>, IEmployeeRepository
    {
        public bool IsLoggedIn(int employeeID)
        {
            using (var sc = new SqlConnection(Context.Database.Connection.ConnectionString))
            {
                return sc.Query<bool>(@"
                            select 1
                            from EmployeeTimers et
                            join EmployeeTimerEntries ete on et.EmployeeTimerID = ete.EmployeeTimerID
                            where et.EmployeeID = @employeeID
                            and ete.ClockOut is null
                            and abs(datediff(day, et.Day, getdate())) <= 1", new { employeeID }).Any();
            }
        }

        public Employee Find(string username)
        {
            var queryEmployee = @"
declare @employeeid int = (select top 1 EmployeeID from Employees where Username = @username)
select e.EmployeeID ID, e.*
from Employees e
where EmployeeID = @employeeid";

            var queryRoles = @"
select r.RoleID ID, r.Name
from Roles r
join EmployeeRoles er on r.RoleID = er.RoleId
where @employeeid = er.EmployeeID";

            var queryDepartments = @"
select d.DepartmentID ID, d.*
from Departments d
join EmployeeDepartments ed on d.DepartmentID = ed.DepartmentID
where @employeeid = ed.EmployeeID";

            var employee = Context.Database.Connection.Query<Employee>(queryEmployee, new { username }).FirstOrDefault();
            var roles = Context.Database.Connection.Query<Role>(queryRoles, new { employeeid = employee.ID });
            var depts = Context.Database.Connection.Query<Department>(queryDepartments, new { employeeid = employee.ID });

            employee.Roles = roles.ToList();
            employee.Departments = depts.ToList();

            return employee;
        }

        public IQueryable<Employee> GetEmployeeListings(int currentUserId)
        {
            using (var sc = new SqlConnection(Context.Database.Connection.ConnectionString))
            {
                var EmployeeListQuery = Resources.GetEmployeeListings;
                var EmployeeList = sc.QueryMultiple(EmployeeListQuery, new
                {
                    currentUserId
                });

                var EmployeeListings = EmployeeList.Read<Employee>().AsQueryable().ToList();
                var AllEmployees = AllIncluding(x => x.Supervisors, x => x.Roles, x => x.Departments, x => x.ClockedInStatus).ToList();
                
                return EmployeeListings.Join(AllEmployees, x => x.ID, y => y.ID, (x, y) => y).AsQueryable();
            }
        }

        #region UserRoles
        private IQueryable<Role> _roleEntities = null;

        public IQueryable<Role> GetRolesForUser(int employeeID)
        {
            return _roleEntities = _roleEntities ?? Context.Employees.Where(u => u.ID == employeeID).SelectMany(u => u.Roles);
        }

        public Role AddRole(int employeeID, Role entity)
        {
            var existingEmployee = Context.Employees.Include("Roles").FirstOrDefault(u => u.ID == employeeID);
            if (Context.Entry(entity).State == System.Data.Entity.EntityState.Detached)
                Context.Roles.Attach(entity);

            existingEmployee?.Roles.Add(entity);
            Save();
            return entity;
        }

        public void RemoveRole(int employeeID, int roleID)
        {
            var existingEmployee = Context.Employees.Include("Roles").FirstOrDefault(u => u.ID == employeeID);
            var roleItem = Context.Roles.FirstOrDefault(d => d.ID == roleID);
            existingEmployee?.Roles.Remove(roleItem);
            Save();
        }

        public bool EmployeeRoleExists(int id, int roleID)
        {
            return Context.Employees.First(u => u.ID == id).Roles.Any(d => d.ID == roleID);
        }

        public bool IsEmployeeSuperintendent(int employeeID)
        {
            return Context.Employees.First(x => x.ID == employeeID).Roles.Any(r => r.Name == ProjectConstants.SuperintendentRole);
        }
        #endregion

        #region EmployeeDepartments
        private IQueryable<Department> _departmentEntities = null;

        public IQueryable<Department> GetDepartmentsForEmployee(int employeeID)
        {
            return _departmentEntities = _departmentEntities ?? Context.Employees.Where(u => u.ID == employeeID).SelectMany(u => u.Departments);
        }

        public Department AddDepartment(int employeeID, Department entity)
        {
            var existingEmployee = Context.Employees.Include("Departments").FirstOrDefault(u => u.ID == employeeID);
            if (Context.Entry(entity).State == System.Data.Entity.EntityState.Detached)
                Context.Departments.Attach(entity);

            existingEmployee.Departments.Add(entity);
            Save();
            return entity;
        }

        public void RemoveDepartment(int employeeID, int departmentID)
        {
            var existingEmployee = Context.Employees.Include("Departments").FirstOrDefault(u => u.ID == employeeID);
            var departmentItem = Context.Departments.FirstOrDefault(d => d.ID == departmentID);
            existingEmployee?.Departments.Remove(departmentItem);
            Save();
        }
        #endregion

        #region EmployeeSupervisors
        private IQueryable<Employee> _supervisorEntities = null;

        public IQueryable<Employee> GetDirectSupervisorsForEmployee(int employeeID)
        {
            return _supervisorEntities = _supervisorEntities ?? Context.Employees.Where(u => u.ID == employeeID).SelectMany(u => u.Supervisors);
        }

        public IEnumerable<EmployeeDailyTimesheetDto> GetDailySupervisorsList(DateTime day, int departmentID, int employeeID)
        {
            using (var sc = new SqlConnection(Context.Database.Connection.ConnectionString))
            {
                sc.Open();
                return
                    sc.Query<EmployeeDailyTimesheetDto>(
                        @"with SupervisorsAndSuperintendents as
                            (
                                select SupervisorID, EmployeeID
                                from EmployeeSupervisors
                                union
                                select er.EmployeeID, er.EmployeeID
                                from EmployeeRoles er
                                where er.RoleID = 8

                            ), AllSupervisors as
                            (
                                select s.EmployeeID, s.Name, max(et.Day) LastDaySupervised
                                from SupervisorsAndSuperintendents es
                                join Employees s on es.SupervisorID = s.EmployeeID
                                left join Timesheets t on s.EmployeeID = t.EmployeeID
                                left join EmployeeTimers et on t.TimesheetID = et.TimesheetID and et.EmployeeID = @employeeID
                                group by s.EmployeeID, s.Name
                            )
                            
                                select s.EmployeeID, s.Name, t.TimesheetID
                                from AllSupervisors s
                                left join SupervisorsAndSuperintendents es on s.EmployeeID = es.SupervisorID and es.EmployeeID = @EmployeeID
                                join Timesheets t on s.EmployeeID = t.EmployeeID and t.Day = @day and t.DepartmentID = @departmentID
                                order by LastDaySupervised desc, case when es.EmployeeID = @employeeID then 1 else 0 end desc, [Name]", new { day, departmentID, employeeID });
            }
        }

        public Employee AddSupervisor(int employeeID, Employee entity)
        {
            var existingEmployee = Context.Employees.Include("Supervisors").FirstOrDefault(u => u.ID == employeeID);
            if (Context.Entry(entity).State == System.Data.Entity.EntityState.Detached)
                Context.Employees.Attach(entity);

            existingEmployee?.Supervisors.Add(entity);
            Save();
            return entity;
        }

        public void RemoveSupervisor(int employeeID, int supervisorID)
        {
            var existingEmployee = Context.Employees.Include("Supervisors").FirstOrDefault(u => u.ID == employeeID);
            var supervisorItem = Context.Employees.FirstOrDefault(d => d.ID == supervisorID);
            existingEmployee?.Supervisors.Remove(supervisorItem);
            Save();
        }
        #endregion
    }

    public interface IEmployeeRepository : IGenericRepository<Employee>
    {
        bool IsLoggedIn(int employeeID);
        IQueryable<Role> GetRolesForUser(int id);
        Employee Find(string username);
        Role AddRole(int employeeID, Role entity);
        void RemoveRole(int employeeID, int roleID);
        bool EmployeeRoleExists(int employeeID, int roleID);
        IQueryable<Employee> GetEmployeeListings(int currentEmployeeID);

        IQueryable<Department> GetDepartmentsForEmployee(int employeeID);
        IEnumerable<EmployeeDailyTimesheetDto> GetDailySupervisorsList(DateTime day, int departmentID, int employeeID);
        Department AddDepartment(int employeeID, Department entity);
        void RemoveDepartment(int employeeID, int departmentID);

        IQueryable<Employee> GetDirectSupervisorsForEmployee(int employeeID);
        Employee AddSupervisor(int employeeID, Employee entity);
        void RemoveSupervisor(int employeeID, int supervisorID);
        bool IsEmployeeSuperintendent(int employeeID);

    }
}
