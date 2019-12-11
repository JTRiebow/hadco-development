namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ClockInAndOutEmployeeAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EmployeeTimerEntries", "ClockInEmployeeID", c => c.Int());
            AddColumn("dbo.EmployeeTimerEntries", "ClockOutEmployeeID", c => c.Int());
            CreateIndex("dbo.EmployeeTimerEntries", "ClockInEmployeeID");
            CreateIndex("dbo.EmployeeTimerEntries", "ClockOutEmployeeID");
            AddForeignKey("dbo.EmployeeTimerEntries", "ClockInEmployeeID", "dbo.Employees", "EmployeeID");
            AddForeignKey("dbo.EmployeeTimerEntries", "ClockOutEmployeeID", "dbo.Employees", "EmployeeID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.EmployeeTimerEntries", "ClockOutEmployeeID", "dbo.Employees");
            DropForeignKey("dbo.EmployeeTimerEntries", "ClockInEmployeeID", "dbo.Employees");
            DropIndex("dbo.EmployeeTimerEntries", new[] { "ClockOutEmployeeID" });
            DropIndex("dbo.EmployeeTimerEntries", new[] { "ClockInEmployeeID" });
            DropColumn("dbo.EmployeeTimerEntries", "ClockOutEmployeeID");
            DropColumn("dbo.EmployeeTimerEntries", "ClockInEmployeeID");
        }
    }
}
