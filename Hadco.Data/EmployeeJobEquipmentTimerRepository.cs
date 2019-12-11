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
    public class EmployeeJobEquipmentTimerRepository : GenericRepository<EmployeeJobEquipmentTimer>, IEmployeeJobEquipmentTimerRepository
    {               
        
    }

    public interface IEmployeeJobEquipmentTimerRepository : IGenericRepository<EmployeeJobEquipmentTimer>
    {
        
    }
}
