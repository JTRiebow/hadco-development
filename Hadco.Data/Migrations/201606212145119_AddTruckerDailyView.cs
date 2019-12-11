namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddTruckerDailyView : DbMigration
    {
        public override void Up()
        {
            Sql(@"
            --Set the options to support indexed views.  
            SET NUMERIC_ROUNDABORT OFF;  
            SET ANSI_PADDING, ANSI_WARNINGS, CONCAT_NULL_YIELDS_NULL, ARITHABORT,  
                QUOTED_IDENTIFIER, ANSI_NULLS ON;  
            GO  
            --Create view with schemabinding.  
            IF OBJECT_ID ('TruckerDailies', 'view') IS NOT NULL  
            DROP VIEW TruckerDailies;  
            GO  
            CREATE VIEW TruckerDailies  
            WITH SCHEMABINDING  
            AS  
            
            select  t.Day as [Date], 
            		e.Name, 
            		lt.LoadTimerID, 
            		lt.TicketNumber, 
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
                    cast(cast(
            				sum(isnull(datediff(second, lte.StartTime, lte.EndTime), 0)) 
            				- sum(isnull(datediff(second, d.StartTime, d.StopTime), 0)) 
            			as decimal)/3600 as decimal(8, 2)) as TotalHours,
            		lt.CalculatedRevenue
            from dbo.LoadTimers lt
            join dbo.LoadTimerEntries lte on lt.LoadTimerID = lte.LoadTimerID
            join dbo.Timesheets t on lt.TimesheetID = t.TimesheetID
            join dbo.Employees e on t.EmployeeID = e.EmployeeID
            left join dbo.Equipment tr on lt.TruckID = tr.EquipmentID
            left join dbo.Equipment tlr on lt.TrailerID = tlr.EquipmentID
            left join dbo.Equipment pp on lt.PupID = pp.EquipmentID
            left join dbo.Equipment le on lt.LoadEquipmentID = le.EquipmentID
            join dbo.Materials m on lt.MaterialID = m.MaterialID
            join dbo.Jobs j on lt.JobID = j.JobID
            join dbo.Phases ph on lt.PhaseID = ph.PhaseID
            join dbo.Categories c on lt.CategoryID = c.CategoryID
            join dbo.BillTypes b on lt.BillTypeID = b.BillTypeID
            left join dbo.DowntimeTimers d on lt.LoadTimerID = d.LoadTimerID
            
            group by
            	t.Day, 
            		e.Name, 
            		lt.LoadTimerID, 
            		lt.TicketNumber, 
                    tr.EquipmentNumber, 
            		tlr.EquipmentNumber, 
            		pp.EquipmentNumber, 
                    lt.Tons, lt.StartLocation, 
            		lt.EndLocation, 
            		b.Name, 
            		lt.Note, 
            		lt.InvoiceNumber, 
            		lt.BillTypeID,
            		lt.CalculatedRevenue,
            		m.Name, 
            		le.EquipmentNumber,
                    j.JobNumber, j.JobID, 
            		ph.PhaseNumber, ph.PhaseID, 
            		c.CategoryNumber, c.CategoryID,  
            		e.[Location],
                    t.DepartmentID;
            ");

        }

        public override void Down()
        {
            Sql(@"drop view dbo.TruckerDailies");
        }
    }
}
