using Hadco.Data.Entities;
using Hadco.Common.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace Hadco.Data
{
    public class OccurrenceRepository : GenericRepository<Occurrence>, IOccurrenceRepository
    {
        
    }

    public interface IOccurrenceRepository : IGenericRepository<Occurrence>
    {

    }
}
