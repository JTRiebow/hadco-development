using Hadco.Data.Entities;
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
    public class TruckClassificationRepository : GenericRepository<TruckClassification>, ITruckClassificationRepository
    {
        public int? GetTruckClassificationID(int? truckID, int? trailerID, int? pupID)
        {
            using (var sc = new SqlConnection(Context.Database.Connection.ConnectionString))
            {
               sc.Open();
                return sc.Query<int>(Resources.GetTruckClassificationID, new {TruckID = truckID, TrailerID = trailerID, PupID = pupID}).SingleOrDefault();
            }
        }
    }

    public interface ITruckClassificationRepository : IGenericRepository<TruckClassification>
    {
        int? GetTruckClassificationID(int? truckID, int? trailerID, int? pupID);
    }
}
