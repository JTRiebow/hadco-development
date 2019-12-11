using System;
using Hadco.Data.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Diagnostics;
using Dapper;
using Hadco.Common.DataTransferObjects;
using Hadco.Common;
using EntityFramework.Extensions;
using Hadco.Common.Enums;

namespace Hadco.Data
{
    public class PricingRepository : GenericRepository<Pricing>, IPricingRepository
    {
        public dynamic GetHistoryMaterials(int customerTypeID, int billTypeID, int? jobID, int? customerID, int? phaseID)
        {
            using (var sc = new SqlConnection(Context.Database.Connection.ConnectionString))
            {
                sc.Open();

                var filter = CreateFilter(customerTypeID, jobID, customerID, phaseID);

                var sql =
                    $@"declare @sql AS NVARCHAR(max);

            SET @sql = N'select *
            FROM
            (
                select  pr.PricingID, p.Value,
                        coalesce(c.CustomerNumber, j.JobNumber, ph.PhaseNumber) Name, m.Name MaterialName, pr.UpdatedTime,
                        convert(VARCHAR(10), pr.StartDate, 101) As StartDate, convert(VARCHAR(10), pr.StartDate, 101) As EndDate

                from Pricings pr
                    left join prices as p on pr.PricingID = P.PricingID
                    left join jobs as j on j.JobID = pr.JobID
                    left join customers as c on c.CustomerID = pr.CustomerID
                    left join Phases as ph on pr.PhaseID = ph.PhaseID
                    right join Materials as m on m.MaterialID = p.MaterialID
                    where pr.CustomerTypeID = {customerTypeID} and pr.BillTypeID = {billTypeID}
                    {filter}
            ) as p

            Pivot(Max(p.Value) FOR materialName IN('  +
                STUFF(
                     (SELECT N',' + QUOTENAME(materialNames) AS[text()]
                        FROM(SELECT DISTINCT m.Name AS materialNames
                           FROM Materials m) AS Prices
                     ORDER BY materialNames
                     FOR XML PATH('')), 1, 1, '') + N')) AS T;';

            EXEC sp_executesql @stmt = @sql;";

                return sc.Query(sql);
            }
        }

        public dynamic GetHistoryTrucks(int customerTypeID, int billTypeID, int? jobID, int? customerID, int? phaseID)
        {
            using (var sc = new SqlConnection(Context.Database.Connection.ConnectionString))
            {
                sc.Open();

                var filter = CreateFilter(customerTypeID, jobID, customerID, phaseID);

                var sql =
                    $@"declare @sql AS NVARCHAR(max);

            SET @sql = N'select *
            FROM
            (
                select  pr.PricingID, p.Value,
                        coalesce(c.CustomerNumber, j.JobNumber, ph.PhaseNumber) Name, t.Name Truck, pr.UpdatedTime,
                        convert(VARCHAR(10), pr.StartDate, 101) As StartDate, convert(VARCHAR(10), pr.EndDate, 101) As EndDate

                from Pricings pr
                    left join prices as p on pr.PricingID = P.PricingID
                    left join jobs as j on j.JobID = pr.JobID
                    left join customers as c on c.CustomerID = pr.CustomerID
                    left join phases as ph on ph.PhaseID = pr.PhaseID
                    right join TruckClassifications as t on t.TruckClassificationID = p.TruckClassificationID
                    where pr.CustomerTypeID = {customerTypeID} and pr.BillTypeID = {billTypeID}
                    {filter}
            ) as p

            Pivot(Max(p.Value) FOR Truck IN('  +
                STUFF(
                     (SELECT N',' + QUOTENAME(truckNames) AS[text()]
                        FROM(SELECT DISTINCT t.Name AS truckNames
                           FROM TruckClassifications t) AS Prices
                     ORDER BY truckNames
                     FOR XML PATH('')), 1, 1, '') + N')) AS T;';

            EXEC sp_executesql @stmt = @sql;";

                return sc.Query(sql);
            }
        }

        public dynamic GetTruckPrices(int customerTypeID, int billTypeID)
        {
            using (var sc = new SqlConnection(Context.Database.Connection.ConnectionString))
            {
                sc.Open();
                string selectAndJoins;
                switch (customerTypeID)
                {
                    case (int) CustomerTypeName.Development:
                        selectAndJoins = @" select pr.PricingID,
                                        p.Value, 
                                        j.JobNumber, 
                                        tc.Name TruckName,  
                                        convert(VARCHAR(10), pr.StartDate, 101) As StartDate,
                                        convert(VARCHAR(10), pr.EndDate, 101) As EndDate,
                                        pr.UpdatedTime
                                        from
					                    (
					                        select t.JobNumber, t.JobID, t.BillTypeID, t.StartDate, max(pr.UpdatedTime) UpdatedTime
					                        from (
					                        	select j.JobNumber, j.JobID, pr.BillTypeID, max(pr.StartDate) StartDate
					                        	from Pricings pr                                        				
					                        	join Jobs j on pr.JobID = j.JobID
					                        	where pr.StartDate <= GETDATE()
					                        	group by j.JobNumber, j.JobID, pr.BillTypeID
					                        ) t
					                        join Pricings pr on t.JobID = pr.JobID and t.BillTypeID = pr.BillTypeID and t.StartDate = pr.StartDate
					                        group by t.JobNumber, t.JobID, t.BillTypeID, t.StartDate
					                    ) j	
					                    join Pricings pr on j.JobID = pr.JobID and j.BillTypeID = pr.BillTypeID and j.UpdatedTime = pr.UpdatedTime";
                        break;
                    case (int) CustomerTypeName.Residential:
                        selectAndJoins = @"select pr.PricingID, 
                                        p.Value,
                                        c.CustomerNumber, 
                                        tc.Name TruckName, 
                                        convert(VARCHAR(10), pr.StartDate, 101) As StartDate,
                                        convert(VARCHAR(10), pr.EndDate, 101) As EndDate,
                                        pr.UpdatedTime
                                        from
					                    (
					                    	select t.CustomerNumber, t.CustomerID, t.BillTypeID, t.StartDate, max(pr.UpdatedTime) UpdatedTime
					                    	from (
					                    		select c.CustomerNumber, c.CustomerID, pr.BillTypeID, max(pr.StartDate) StartDate
					                    		from Pricings pr                                        				
					                    		join Customers c on pr.CustomerID = c.CustomerID
					                    		where pr.StartDate <= GETDATE()
					                    		group by c.CustomerNumber, c.CustomerID, pr.BillTypeID
					                    	) t
					                    	join Pricings pr on t.CustomerID = pr.CustomerID and t.BillTypeID = pr.BillTypeID and t.StartDate = pr.StartDate
					                    	group by t.CustomerNumber, t.CustomerID, t.BillTypeID, t.StartDate
					                    ) c	
					                    join Pricings pr on c.CustomerID = pr.CustomerID and c.BillTypeID = pr.BillTypeID and c.UpdatedTime = pr.UpdatedTime";
                        break;
                    case (int) CustomerTypeName.Outside:
                        selectAndJoins = @"select pr.PricingID, 
                                        p.Value,
                                        ph.PhaseNumber CustomerNumber, 
                                        tc.Name TruckName, 
                                        convert(VARCHAR(10), pr.StartDate, 101) As StartDate,
                                        convert(VARCHAR(10), pr.EndDate, 101) As EndDate,
                                        pr.UpdatedTime
                                        from
					                    (
					                    	select t.PhaseNumber, t.PhaseID, t.BillTypeID, t.StartDate, max(pr.UpdatedTime) UpdatedTime
					                    	from (
					                    		select ph.PhaseNumber, ph.PhaseID, pr.BillTypeID, max(pr.StartDate) StartDate
					                    		from Pricings pr                                        				
					                    		join Phases ph on pr.PhaseID = ph.PhaseID
					                    		where pr.StartDate <= GETDATE()
					                    		group by ph.PhaseNumber, ph.PhaseID, pr.BillTypeID
					                    	) t
					                    	join Pricings pr on t.PhaseID = pr.PhaseID and t.BillTypeID = pr.BillTypeID and t.StartDate = pr.StartDate
					                    	group by t.PhaseNumber, t.PhaseID, t.BillTypeID, t.StartDate
					                    ) ph	
					                    join Pricings pr on ph.PhaseID = pr.PhaseID and ph.BillTypeID = pr.BillTypeID and ph.UpdatedTime = pr.UpdatedTime";
                        break;
                    default:
                        return null;
                }
                var sql =
                    $@"declare @sql AS NVARCHAR(max);

                                        SET @sql = N'select *
                                        			from
                                        			(
                                        	{selectAndJoins}
                                            left join prices as p on pr.PricingID = P.PricingID 
                                        	right join TruckClassifications as tc on tc.TruckClassificationID = p.TruckClassificationID 
                                        	    where pr.CustomerTypeID = {customerTypeID} and pr.BillTypeID = {billTypeID}
                                        				
                                        			) as p
                                        			
                                        			Pivot( Max(p.Value) FOR truckName IN('  +
                                        
                                        STUFF(
                                            (SELECT N',' + QUOTENAME(truckNames) AS [text()]
                                             FROM (SELECT DISTINCT tc.Name AS truckNames
                                                   FROM TruckClassifications tc ) AS Prices
                                             ORDER BY TruckNames
                                             FOR XML PATH('')), 1, 1, '') + N')) AS T order by UpdatedTime desc;';
                                        
                                        EXEC sp_executesql @stmt = @sql;";

                return sc.Query<dynamic>(sql);
            }
        }

        public dynamic GetMaterialPrices(int customerTypeID, int billTypeID)
        {
            using (var sc = new SqlConnection(Context.Database.Connection.ConnectionString))
            {
                sc.Open();
                string selectAndJoins;
                switch (customerTypeID)
                {
                    case (int) CustomerTypeName.Development:
                        selectAndJoins = @" select pr.PricingID,
                                        p.Value,  
                                        j.JobNumber, 
                                        m.Name MaterialName, 
                                        convert(VARCHAR(10), pr.StartDate, 101) As StartDate,
                                        convert(VARCHAR(10), pr.EndDate, 101) As EndDate,
                                        pr.UpdatedTime
                                        from
					                    (
					                        select t.JobNumber, t.JobID, t.BillTypeID, t.StartDate, max(pr.UpdatedTime) UpdatedTime
					                        from (
					                        	select j.JobNumber, j.JobID, pr.BillTypeID, max(pr.StartDate) StartDate
					                        	from Pricings pr                                        				
					                        	join Jobs j on pr.JobID = j.JobID
					                        	where pr.StartDate <= GETDATE()
					                        	group by j.JobNumber, j.JobID, pr.BillTypeID
					                        ) t
					                        join Pricings pr on t.JobID = pr.JobID and t.BillTypeID = pr.BillTypeID and t.StartDate = pr.StartDate
					                        group by t.JobNumber, t.JobID, t.BillTypeID, t.StartDate
					                    ) j	
					                    join Pricings pr on j.JobID = pr.JobID and j.BillTypeID = pr.BillTypeID and j.UpdatedTime = pr.UpdatedTime";
                        break;
                    case (int) CustomerTypeName.Residential:
                        selectAndJoins = @" select pr.PricingID, 
                                        p.Value,
                                        c.CustomerNumber, 
                                        m.Name MaterialName, 
                                        convert(VARCHAR(10), pr.StartDate, 101) As StartDate,
                                        convert(VARCHAR(10), pr.EndDate, 101) As EndDate,
                                        pr.UpdatedTime
from
					                    (
					                    	select t.CustomerNumber, t.CustomerID, t.BillTypeID, t.StartDate, max(pr.UpdatedTime) UpdatedTime
					                    	from (
					                    		select c.CustomerNumber, c.CustomerID, pr.BillTypeID, max(pr.StartDate) StartDate
					                    		from Pricings pr                                        				
					                    		join Customers c on pr.CustomerID = c.CustomerID
					                    		where pr.StartDate <= GETDATE()
					                    		group by c.CustomerNumber, c.CustomerID, pr.BillTypeID
					                    	) t
					                    	join Pricings pr on t.CustomerID = pr.CustomerID and t.BillTypeID = pr.BillTypeID and t.StartDate = pr.StartDate
					                    	group by t.CustomerNumber, t.CustomerID, t.BillTypeID, t.StartDate
					                    ) c	
					                    join Pricings pr on c.CustomerID = pr.CustomerID and c.BillTypeID = pr.BillTypeID and c.UpdatedTime = pr.UpdatedTime";
                        break;
                    case (int)CustomerTypeName.Outside:
                        selectAndJoins = @" select pr.PricingID, 
                                        p.Value,
                                        ph.PhaseNumber CustomerNumber, 
                                        m.Name MaterialName, 
                                        convert(VARCHAR(10), pr.StartDate, 101) As StartDate,
                                        convert(VARCHAR(10), pr.EndDate, 101) As EndDate,
                                        pr.UpdatedTime
                                         from
					                    (
					                    	select t.PhaseNumber, t.PhaseID, t.BillTypeID, t.StartDate, max(pr.UpdatedTime) UpdatedTime
					                    	from (
					                    		select ph.PhaseNumber, ph.PhaseID, pr.BillTypeID, max(pr.StartDate) StartDate
					                    		from Pricings pr                                        				
					                    		join Phases ph on pr.PhaseID = ph.PhaseID
					                    		where pr.StartDate <= GETDATE()
					                    		group by ph.PhaseNumber, ph.PhaseID, pr.BillTypeID
					                    	) t
					                    	join Pricings pr on t.PhaseID = pr.PhaseID and t.BillTypeID = pr.BillTypeID and t.StartDate = pr.StartDate
					                    	group by t.PhaseNumber, t.PhaseID, t.BillTypeID, t.StartDate
					                    ) ph	
					                    join Pricings pr on ph.PhaseID = pr.PhaseID and ph.BillTypeID = pr.BillTypeID and ph.UpdatedTime = pr.UpdatedTime";
                        break;
                    case (int) CustomerTypeName.Metro:
                        selectAndJoins = @" select pr.PricingID, 
                                        p.Value,
                                        ph.PhaseNumber RunNumber, 
                                        m.Name MaterialName, 
                                        convert(VARCHAR(10), pr.StartDate, 101) As StartDate,
                                        convert(VARCHAR(10), pr.EndDate, 101) As EndDate,
                                        pr.UpdatedTime
                                         from
					                    (
					                    	select t.PhaseNumber, t.PhaseID, t.BillTypeID, t.StartDate, max(pr.UpdatedTime) UpdatedTime
					                    	from (
					                    		select ph.PhaseNumber, ph.PhaseID, pr.BillTypeID, max(pr.StartDate) StartDate
					                    		from Pricings pr                                        				
					                    		join Phases ph on pr.PhaseID = ph.PhaseID
					                    		where pr.StartDate <= GETDATE()
					                    		group by ph.PhaseNumber, ph.PhaseID, pr.BillTypeID
					                    	) t
					                    	join Pricings pr on t.PhaseID = pr.PhaseID and t.BillTypeID = pr.BillTypeID and t.StartDate = pr.StartDate
					                    	group by t.PhaseNumber, t.PhaseID, t.BillTypeID, t.StartDate
					                    ) ph	
					                    join Pricings pr on ph.PhaseID = pr.PhaseID and ph.BillTypeID = pr.BillTypeID and ph.UpdatedTime = pr.UpdatedTime";
                        break;
                    default:
                        return null;
                }
                var sql =
                    $@"declare @sql AS NVARCHAR(max);
                                        SET @sql = N'select *
                                        			from
                                        			(
                                                     {selectAndJoins}
                                                        left join prices p on pr.PricingID = P.PricingID 
                                        				right join Materials m on m.MaterialID = p.MaterialID
                                                        where pr.CustomerTypeID = {customerTypeID} and pr.BillTypeID = {billTypeID}                                      				
                                        			) as p
                                        			
                                        			Pivot( Max(p.Value) FOR materialName IN('  +
                                        
                                        STUFF(
                                            (SELECT N',' + QUOTENAME(materialNames) AS [text()]
                                             FROM (SELECT DISTINCT m.Name AS materialNames
                                                   From Materials m ) AS Prices
                                             ORDER BY materialNames
                                             FOR XML PATH('')), 1, 1, '') + N')) AS T order by UpdatedTime desc';                                    
                                        
                                        EXEC sp_executesql @stmt = @sql;";

                return sc.Query<dynamic>(sql);
            }
        }

        public dynamic GetPricesByID(int pricingID)
        {
            using (var sc = new SqlConnection(Context.Database.Connection.ConnectionString))
            {
                sc.Open();
                var prices = sc.Query(@"select  p.PricingID, p.PriceID as ID, p.Value,
                                                t.Name as Truck, t.TruckClassificationID,
                                                m.Name as Material, m.MaterialID
                                    	from Prices p
                                    left join TruckClassifications t on p.TruckClassificationID = t.TruckClassificationID
                                    left join Materials m on p.MaterialID = m.MaterialID
                                    where p.PricingID = @pricingID"
                    , new {pricingID});
                return prices;
            }
        }

        public dynamic GetJobPricingList(int customerTypeID, int billTypeID)
        {
            using (var sc = new SqlConnection(Context.Database.Connection.ConnectionString))
            {
                sc.Open();
                var query = @"select j.JobID, j.JobNumber Name, p.PricingID
                                    from Jobs j
                                    left join Pricings p on p.JobID = j.JobID
                                    where j.Status = 1
                                    and (p.CustomerTypeID is null or p.CustomerID = @customerTypeID)
                                    and (p.BillTypeID is null or p.BillTypeID = @billTypeID);";

                var list = sc.Query(query, new {customerTypeID, billTypeID});
                return list;
            }
        }

        public dynamic GetCustomerPricingList(int customerTypeID, int billTypeID)
        {
            using (var sc = new SqlConnection(Context.Database.Connection.ConnectionString))
            {
                sc.Open();
                var query = @"select c.CustomerID, c.CustomerNumber Name, p.PricingID
                                    from Customers c
                                    left join Pricings p on p.CustomerID = c.CustomerID
                                    where (p.CustomerTypeID is null or p.CustomerTypeID = @customerTypeID)
                                    and (p.BillTypeID is null or p.BillTypeID = @billTypeID);";

                var list = sc.Query(query, new {customerTypeID, billTypeID});
                return list;
            }
        }

        public dynamic GetPhasePricingList(int customerTypeID, int billTypeID)
        {
            string jobNumber = null;
            if (customerTypeID == (int)CustomerTypeName.Metro)
            {
                jobNumber = "METRO";
            }
            if (customerTypeID == (int)CustomerTypeName.Outside)
            {
                jobNumber = "TKMISC";
            }
            using (var sc = new SqlConnection(Context.Database.Connection.ConnectionString))
            {
                sc.Open();
                var query = @"select ph.PhaseID, ph.PhaseNumber Name, p.PricingID
                                    from Phases ph
                                    left join Pricings p on p.PhaseID = ph.PhaseID
                                    where ph.JobNumber = @jobNumber
                                    and (p.CustomerTypeID is null or p.CustomerTypeID = @customerTypeID)
                                    and (p.BillTypeID is null or p.BillTypeID = @billTypeID)";

                var list = sc.Query(query, new {jobNumber, customerTypeID, billTypeID});
                return list;
            }
        }

        private static string CreateFilter(int customerTypeID, int? jobID, int? customerID, int? phaseID)
        {
            switch (customerTypeID)
            {
                case (int) CustomerTypeName.Development:
                    return $"and pr.jobID = {jobID}";
                case (int) CustomerTypeName.Residential:
                    return $"and pr.customerID = {customerID}";
            }
            return $"and pr.PhaseID = {phaseID}";
        }
    }

    public interface IPricingRepository : IGenericRepository<Pricing>
    {
        dynamic GetTruckPrices(int customerTypeID, int billTypeID);
        dynamic GetMaterialPrices(int customerTypeID, int billTypeID);
        dynamic GetPricesByID(int pricingID);
        dynamic GetHistoryMaterials(int customerTypeID, int billTypeID, int? jobID, int? customerID, int? phaseID);
        dynamic GetHistoryTrucks(int customerTypeID, int billTypeID, int? jobID, int? customerID, int? phaseID);
        dynamic GetJobPricingList(int customerTypeID, int billTypeID);
        dynamic GetCustomerPricingList(int customerTypeID, int billTypeID);
        dynamic GetPhasePricingList(int customerTypeID, int billTypeID);
    }
}