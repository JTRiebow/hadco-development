using Hadco.Data.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Hadco.Common.DataTransferObjects;

namespace Hadco.Data
{
    public class EquipmentTimerRepository : GenericRepository<EquipmentTimer>, IEquipmentTimerRepository
    {     
        public override EquipmentTimer FindNoTracking(int id,
            params Expression<Func<EquipmentTimer, object>>[] includeProperties)
        {
            return base.FindNoTracking(id, x => x.EquipmentTimerEntries);

        }
    }

    public interface IEquipmentTimerRepository : IGenericRepository<EquipmentTimer>
    {
    }
}
