using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Hadco.Common.DataTransferObjects;
using Hadco.Data.Entities;
using System.Collections.Immutable;
using System.Data.Entity;

namespace Hadco.Data
{
    public class RoleAuthActivityRepository : GenericRepository<RoleAuthActivity>, IRoleAuthActivityRepository
    {

        public void AddRoleAuthActivityDepartments(int roleAuthActivityID, IEnumerable<int> departmentIDs)
        {
            using (var sc = new SqlConnection(Context.Database.Connection.ConnectionString))
            {
                foreach(var departmentID in departmentIDs)
                {
                    sc.Execute(@"insert into RoleAuthActivityDepartments (RoleAuthActivityID, DepartmentID) values (@roleAuthActivityID, @departmentID)", new { roleAuthActivityID, departmentID });
                }
            }
        }

        public void RemoveRoleAuthActivityDepartments(int roleAuthActivityID)
        {
            using (var sc = new SqlConnection(Context.Database.Connection.ConnectionString))
            {
                sc.Execute(@"delete from RoleAuthActivityDepartments where RoleAuthActivityID = @roleAuthActivityID", new { roleAuthActivityID });
            }
        }

        public void UpdateRoleAuthActivity(int roleAuthActivityID, bool ownDepartments, bool allDepartments)
        {
            using (var sc = new SqlConnection(Context.Database.Connection.ConnectionString))
            {
                sc.Execute($@"
                        update RoleAuthActivities
                        set OwnDepartments = @ownDepartments,
                        AllDepartments = @allDepartments
                        where RoleAuthActivityID = @roleAuthActivityID
                        ", new { roleAuthActivityID, ownDepartments, allDepartments });
            }
        }

        public override IQueryable<RoleAuthActivity> All
        {
            get { return base.All.Include(x => x.Departments); }
        }
    }

    public interface IRoleAuthActivityRepository : IGenericRepository<RoleAuthActivity>
    {
        void AddRoleAuthActivityDepartments(int roleAuthActivityID, IEnumerable<int> departmentIDs);
        void RemoveRoleAuthActivityDepartments(int roleAuthActivityID);
        void UpdateRoleAuthActivity(int roleAuthActivityID, bool ownDepartments, bool allDepartments);
    }
}
