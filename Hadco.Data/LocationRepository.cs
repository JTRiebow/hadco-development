using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Data.Common;
using System.Configuration;
using System.Data.SqlClient;
using Hadco.Common.DataTransferObjects;
using Hadco.Data.Entities;

namespace Hadco.Data
{
    public class LocationRepository : GenericRepository<Location>, ILocationRepository
    {
        public IEnumerable<GPSCoordinates> Get(int employeeID, DateTimeOffset startTime, DateTimeOffset endTime, int? departmentID)
        {
            var query = @"
select l.[Timestamp], l.Latitude, l.Longitude
from Locations l
join EmployeeTimers et on l.EmployeeTimerID = et.EmployeeTimerID
where et.EmployeeID = @employeeID
 and l.Timestamp between @startTime and @endTime
";

            if (departmentID.HasValue)
            {
                query += " and et.DepartmentID = @departmentID";
            }

            using (var sc = new SqlConnection(Context.Database.Connection.ConnectionString))
            {
                return sc.Query<GPSCoordinates>(query, new {employeeID, startTime, endTime, departmentID});
            }
        }

        public GPSCoordinates GetMostRecentCoordinates(int employeeID)
        {
            using (var sc = new SqlConnection(Context.Database.Connection.ConnectionString))
            {
                return sc.Query<GPSCoordinates>(@"
                            select top 1 [Timestamp], Latitude, Longitude 
                            from
                            ( 
                             select l.[TimeStamp], l.Latitude, l.Longitude
                             from Locations l
                             join EmployeeTimers et on l.EmployeeTimerID = et.EmployeeTimerID
                              where et.EmployeeID = @employeeID
                             union
                             select ete.ClockIn, ClockInLatitude, ClockInLongitude
                             from EmployeeTimers et
                             join EmployeeTimerEntries ete on et.EmployeeTimerID = ete.EmployeeTimerID
                             where et.EmployeeID = @employeeID
                             and (ClockInLatitude is not null and ClockInLongitude is not null)
                             union
                             select ete.ClockOut, ClockOutLatitude, ClockOutLongitude
                             from EmployeeTimers et
                             join EmployeeTimerEntries ete on et.EmployeeTimerID = ete.EmployeeTimerID
                             where et.EmployeeID = @employeeID
                             and (ClockOutLatitude is not null and ClockOutLongitude is not null)
                            ) as gps
                            order by [Timestamp] desc", new { employeeID }).FirstOrDefault();
            }
        }
    }

    public interface ILocationRepository : IGenericRepository<Location>
    {
        IEnumerable<GPSCoordinates> Get(int employeeID, DateTimeOffset startTime, DateTimeOffset endTime, int? departmentID);
        GPSCoordinates GetMostRecentCoordinates(int employeeID);
    }
}