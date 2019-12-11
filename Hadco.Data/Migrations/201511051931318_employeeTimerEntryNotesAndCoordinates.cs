namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class employeeTimerEntryNotesAndCoordinates : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EmployeeTimerEntries", "ClockInNote", c => c.String(maxLength: 256));
            AddColumn("dbo.EmployeeTimerEntries", "ClockOutNote", c => c.String(maxLength: 256));
            AddColumn("dbo.EmployeeTimerEntries", "Latitude", c => c.Decimal(precision: 9, scale: 6));
            AddColumn("dbo.EmployeeTimerEntries", "Longitude", c => c.Decimal(precision: 9, scale: 6));
        }
        
        public override void Down()
        {
            DropColumn("dbo.EmployeeTimerEntries", "Longitude");
            DropColumn("dbo.EmployeeTimerEntries", "Latitude");
            DropColumn("dbo.EmployeeTimerEntries", "ClockOutNote");
            DropColumn("dbo.EmployeeTimerEntries", "ClockInNote");
        }
    }
}
