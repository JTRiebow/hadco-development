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

namespace Hadco.Data
{
    public class EmployeeTimerEntryRepository : GenericRepository<EmployeeTimerEntry>, IEmployeeTimerEntryRepository
    {               

    }

    public interface IEmployeeTimerEntryRepository : IGenericRepository<EmployeeTimerEntry>
    {

    }
}
