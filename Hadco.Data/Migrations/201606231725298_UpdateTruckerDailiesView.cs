namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateTruckerDailiesView : DbMigration
    {
        public override void Up()
        {
            Sql(@"IF OBJECT_ID ('TruckerDailies', 'view') IS NOT NULL  
            DROP VIEW TruckerDailies;
            GO
            CREATE VIEW TruckerDailies
            AS
             select t.Day[Date], 
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
                     j.JobNumber as Job,
                     j.JobID, 
                     ph.PhaseNumber as Phase,
                     ph.PhaseID, 
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
             left join dbo.Materials m on lt.MaterialID = m.MaterialID

             left join dbo.Jobs j on lt.JobID = j.JobID

             left join dbo.Phases ph on lt.PhaseID = ph.PhaseID
             left join dbo.Categories c on lt.CategoryID = c.CategoryID
             left join dbo.BillTypes b on lt.BillTypeID = b.BillTypeID
             cross apply (select lte.LoadTimerID, convert(decimal(8, 2),
                                                     sum(isnull(dbo.GetTimeSpanHours(lte.StartTime, lte.EndTime), 0))) TotalHours
                         from LoadTimerEntries lte
                         left
                         join dbo.DowntimeTimers d on lte.LoadTimerID = d.LoadTimerID
                         where lte.LoadTimerID = lt.LoadTimerID
                         group by lte.loadTimerID
                         ) lte1
             union all
             select t.Day[Date], 
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
             		convert(decimal(8, 2), dbo.GetTimeSpanHours(d.StartTime, d.StopTime)), 
             		null
             from LoadTimers lt
             join Timesheets t on lt.TimesheetID = t.TimesheetID
             join Employees e on t.EmployeeID = e.EmployeeID
             join DowntimeTimers d on lt.LoadTimerID = d.LoadTimerID
             join DowntimeReasons dr on d.DowntimeReasonID = dr.DowntimeReasonID
             join Equipment tr on lt.TruckID = tr.EquipmentID
             left join Equipment trl on lt.TrailerID = trl.EquipmentID
             left join Equipment pp on lt.PupID = pp.EquipmentID; ");
        }
        
        public override void Down()
        {
        }
    }
}
