using Hadco.Data.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Hadco.Common.DataTransferObjects;

namespace Hadco.Data
{
    public class EmployeeJobTimerRepository : GenericRepository<EmployeeJobTimer>, IEmployeeJobTimerRepository
    {
        public IEnumerable<EmployeeJobTimerSummaryDto> GetEmployeeSummary(int jobTimerID)
        {
            using (var sc = new SqlConnection(Context.Database.Connection.ConnectionString))
            {
                sc.Open();
                var mapped = sc.QueryMultiple(@"
                                select ejt.EmployeeJobTimerID, e.EmployeeID, e.Name, ejt.LaborMinutes
                                from EmployeeJobTimers ejt
                                join EmployeeTimers et on ejt.EmployeeTimerID = et.EmployeeTimerID
                                join Employees e on et.EmployeeID = e.EmployeeID
                                where JobTimerID = @JobTimerID;

                                select ejet.EmployeeJobEquipmentTimerID, ejet.EmployeeJobTimerID, e.EquipmentID, e.EquipmentNumber, ejet.EquipmentMinutes
                                from EmployeeJobEquipmentTimers ejet
                                join Equipment e on ejet.EquipmentID = e.EquipmentID
                                where ejet.EmployeeJobTimerID in (select EmployeeJobTimerID from EmployeeJobTimers ejt where JobTimerID = @JobTimerID)",
                                new { JobTimerID = jobTimerID }).Map<EmployeeJobTimerSummaryDto, EmployeeJobEquipmentTimerSummaryDto, int>(
                        ejt => ejt.EmployeeJobTimerID,
                        ejet => ejet.EmployeeJobTimerID,
                        (ejt, ejet) => { ejt.EmployeeJobEquipmentTimers = ejet; }
                    );
                return mapped;
            }
        }
    }

    public interface IEmployeeJobTimerRepository : IGenericRepository<EmployeeJobTimer>
    {
        IEnumerable<EmployeeJobTimerSummaryDto> GetEmployeeSummary(int jobTimerID);
    }
}
