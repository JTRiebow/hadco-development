namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ClockedInStatuses : DbMigration
    {
        public override void Up()
        {
            //CreateTable(
            //    "dbo.ClockedInStatus",
            //    c => new
            //        {
            //            EmployeeID = c.Int(nullable: false, identity: true),
            //            IsClockedIn = c.Boolean(nullable: false),
            //        })
            //    .PrimaryKey(t => t.EmployeeID);

            //CreateIndex("dbo.Employees", "EmployeeID");
            //AddForeignKey("dbo.Employees", "EmployeeID", "dbo.ClockedInStatus", "EmployeeID");
            Sql(@"
IF EXISTS (select * 
from sys.indexes 
where name = N'IX_ClockOut')
DROP INDEX IX_ClockOut ON [dbo].EmployeeTimerEntries;

go

CREATE NONCLUSTERED INDEX [IX_ClockOut]
ON [dbo].[EmployeeTimerEntries] ([ClockOut]);

if exists (
select *
from sys.views
where name = N'ClockedInStatus')
drop view ClockedInStatus;

go

create view ClockedInStatus as 
with ClockedInEmployees as
(
select et.EmployeeID
from EmployeeTimers et
join EmployeeTimerEntries ete on et.EmployeeTimerID = ete.EmployeeTimerID
where ete.ClockOut is null
and abs(datediff(day, et.Day, getdate())) <= 1
group by EmployeeID
)
select e.EmployeeID, cast(case when ce.EmployeeID is null then 0 else 1 end as bit) IsClockedIn
from Employees e
left join ClockedInEmployees ce on e.EmployeeID = ce.EmployeeID
");
        }
        
        public override void Down()
        {
            //DropForeignKey("dbo.Employees", "EmployeeID", "dbo.ClockedInStatus");
            //DropIndex("dbo.Employees", new[] { "EmployeeID" });
            //DropTable("dbo.ClockedInStatus");
            Sql(@"
IF EXISTS (select * 
from sys.indexes 
where name = N'IX_ClockOut')
DROP INDEX IX_ClockOut ON [dbo].EmployeeTimerEntries;

go

if exists (
select *
from sys.views
where name = N'ClockedInStatus')
drop view ClockedInStatus;");
        }
    }
}
