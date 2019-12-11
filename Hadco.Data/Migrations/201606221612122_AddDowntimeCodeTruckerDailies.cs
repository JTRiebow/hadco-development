namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDowntimeCodeTruckerDailies : DbMigration
    {
        public override void Up()
        {
            Sql(@"create  index idx_lte_loadtimerid on dbo.LoadTimerEntries (LoadTimerID asc) include (StartTime, EndTime);
                    create index idx_dtt_loadtimerid on dbo.DowntimeTimers (LoadTimerID asc) include (StartTime, StopTime);
                    
                    CREATE INDEX [idx_Employees_EmployeeID] ON [dbo].[Employees] ( [EmployeeID]) INCLUDE ([Name], [Location]);
                    
                    CREATE INDEX [_dta_index_LoadTimers_11_1301579675__K2_K17_K5_K4_K3_K1_K13_K14_K8_9_10_11_12_15_16] ON [dbo].[LoadTimers]
                    (    
                        [TimesheetID] ASC,
                        [LoadEquipmentID] ASC,
                        [PupID] ASC,
                        [TrailerID] ASC,
                        [TruckID] ASC,
                        [LoadTimerID] ASC,
                        [MaterialID] ASC,
                        [BillTypeID] ASC,
                        [CategoryID] ASC
                    )
                    INCLUDE (     [StartLocation],
                        [EndLocation],
                        [TicketNumber],
                        [Tons],
                        [Note],
                        [InvoiceNumber]) WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY];
                        
                    create index idx_timesheet_day on dbo.timesheets (day, departmentid, timesheetid, employeeid);

                    CREATE NONCLUSTERED INDEX [_dta_index_DowntimeTimers_11_661577395__K5_K2_3_4] ON [dbo].[DowntimeTimers]
                    (
                    	[LoadTimerID] ASC,
                    	[DowntimeReasonID] ASC
                    )
                    INCLUDE ( 	[StartTime],
                    	[StopTime]) WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY];");

            Sql(@"IF OBJECT_ID ('TruckerDailies', 'view') IS NOT NULL  
            DROP VIEW TruckerDailies;  
            GO  
            CREATE VIEW TruckerDailies   
            AS  
             select t.Day [Date], 
                     e.Name, 
                     lt.LoadTimerID, 
                     lt.TicketNumber,
                     null DowntimeCode, 
                     tr.EquipmentNumber as Truck, 
                     tlr.EquipmentNumber as Trailer, 
                     pp.EquipmentNumber as PupTrailer, 
                     lt.Tons,
                     lt.StartLocation LoadSite, 
                     lt.EndLocation DumpSite, 
                     b.Name as BillType, 
                     lt.Note, lt.InvoiceNumber, 
                     m.Name as Material, 
                     le.EquipmentNumber as LoadEquipment,
                     c.JobNumber as Job,
                     c.JobID, 
                     c.PhaseNumber as Phase,
                     c.PhaseID, 
                     c.CategoryNumber as Category,
                     c.CategoryID,  
                     e.[Location],
                     t.DepartmentID,
                    lte1.TotalHours,
             	   lt.CalculatedRevenue
             from dbo.LoadTimers lt
             join dbo.Timesheets t on lt.TimesheetID = t.TimesheetID
             join dbo.Employees e on t.EmployeeID = e.EmployeeID
             left join dbo.Equipment tr on lt.TruckID = tr.EquipmentID
             left join dbo.Equipment tlr on lt.TrailerID = tlr.EquipmentID
             left join dbo.Equipment pp on lt.PupID = pp.EquipmentID
             left join dbo.Equipment le on lt.LoadEquipmentID = le.EquipmentID
             join dbo.Materials m on lt.MaterialID = m.MaterialID
             join dbo.Categories c on lt.CategoryID = c.CategoryID
             join dbo.BillTypes b on lt.BillTypeID = b.BillTypeID
             cross apply (select lte.LoadTimerID, convert(decimal(8,2), 
                                                     sum(isnull(dbo.GetTimeSpanHours(lte.StartTime, lte.EndTime), 0)) - 
                                                     sum(isnull(dbo.GetTimeSpanHours(d.StartTime, d.StopTime),0))) TotalHours
                         from LoadTimerEntries lte
                         left join dbo.DowntimeTimers d on lte.LoadTimerID = d.LoadTimerID
                         where lte.LoadTimerID = lt.LoadTimerID
                         group by lte.loadTimerID
                         ) lte1
             union all
             select t.Day [Date], 
             		e.Name, 
             		lt.LoadTimerID, 
             		null, 
             		dr.Code DowntimeCode, 
             		tr.EquipmentNumber, 
             		trl.EquipmentNumber, 
             		pp.EquipmentNumber, 
             		null, null, null, null, null, null, null, null, 
             		dr.JobNumber, null,
             		dr.PhaseNumber, null, 
             		dr.CategoryNumber, null, 
             		e.[Location], 
             		t.DepartmentID, 
             		convert(decimal(8,2), dbo.GetTimeSpanHours(d.StartTime, d.StopTime)), 
             		null
             from LoadTimers lt
             join Timesheets t on lt.TimesheetID = t.TimesheetID
             join Employees e on t.EmployeeID = e.EmployeeID
             join DowntimeTimers d on lt.LoadTimerID = d.LoadTimerID
             join DowntimeReasons dr on d.DowntimeReasonID = dr.DowntimeReasonID
             join Equipment tr on lt.TruckID = tr.EquipmentID
             left join Equipment trl on lt.TrailerID = trl.EquipmentID
             left join Equipment pp on lt.PupID = pp.EquipmentID");
        }
        
        public override void Down()
        {
           Sql("DROP VIEW TruckerDailies;");
        }
    }
}
