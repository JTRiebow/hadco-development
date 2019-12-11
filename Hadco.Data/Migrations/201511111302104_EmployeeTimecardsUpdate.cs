namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EmployeeTimecardsUpdate : DbMigration
    {
        public override void Up()
        {
            Sql(@"insert into EmployeeTimecard
                select distinct et.EmployeeID, et.DepartmentID, convert(date, DATEADD(dd, -(DATEPART(dw, et.Day) - 1), et.Day)), 0, 0, 0
                from EmployeeTimers et
                where not exists(select * from employeetimecard etc where etc.DepartmentID = et.DepartmentID and etc.EmployeeID = et.EmployeeID and etc.StartOfWeek = convert(date, DATEADD(dd, -(DATEPART(dw, et.Day) - 1), et.Day)))
                ");

            Sql(@"update et
                set EmployeeTimecardID = etc.EmployeeTimecardID
                from EmployeeTimers et
                join EmployeeTimecard etc on et.EmployeeID = etc.EmployeeID and et.DepartmentID = etc.DepartmentID and
                 (convert(date, DATEADD(dd, -(DATEPART(dw, et.Day) - 1), et.Day))) = etc.StartOfWeek
                 where et.EmployeeTimecardID is null");

            DropForeignKey("dbo.EmployeeTimers", "EmployeeTimecardID", "dbo.EmployeeTimecard");
            DropIndex("dbo.EmployeeTimers", new[] { "EmployeeTimecardID" });
            AlterColumn("dbo.EmployeeTimers", "EmployeeTimecardID", c => c.Int(nullable: false));
            CreateIndex("dbo.EmployeeTimers", "EmployeeTimecardID");
            AddForeignKey("dbo.EmployeeTimers", "EmployeeTimecardID", "dbo.EmployeeTimecard", "EmployeeTimecardID", cascadeDelete: false);
        }
        
        public override void Down()
        {
            //DropForeignKey("dbo.EmployeeTimers", "EmployeeTimecardID", "dbo.EmployeeTimecard");
            //DropIndex("dbo.EmployeeTimers", new[] { "EmployeeTimecardID" });
            //AlterColumn("dbo.EmployeeTimers", "EmployeeTimecardID", c => c.Int());
            //CreateIndex("dbo.EmployeeTimers", "EmployeeTimecardID");
            //AddForeignKey("dbo.EmployeeTimers", "EmployeeTimecardID", "dbo.EmployeeTimecard", "EmployeeTimecardID");
        }
    }
}
