namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateEmployeeTimerEntryCoordinates : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EmployeeTimerEntries", "ClockInLatitude", c => c.Decimal(precision: 9, scale: 6));
            AddColumn("dbo.EmployeeTimerEntries", "ClockInLongitude", c => c.Decimal(precision: 9, scale: 6));
            AddColumn("dbo.EmployeeTimerEntries", "ClockOutLatitude", c => c.Decimal(precision: 9, scale: 6));
            AddColumn("dbo.EmployeeTimerEntries", "ClockOutLongitude", c => c.Decimal(precision: 9, scale: 6));
            DropColumn("dbo.EmployeeTimerEntries", "Latitude");
            DropColumn("dbo.EmployeeTimerEntries", "Longitude");
        }
        
        public override void Down()
        {
            AddColumn("dbo.EmployeeTimerEntries", "Longitude", c => c.Decimal(precision: 9, scale: 6));
            AddColumn("dbo.EmployeeTimerEntries", "Latitude", c => c.Decimal(precision: 9, scale: 6));
            DropColumn("dbo.EmployeeTimerEntries", "ClockOutLongitude");
            DropColumn("dbo.EmployeeTimerEntries", "ClockOutLatitude");
            DropColumn("dbo.EmployeeTimerEntries", "ClockInLongitude");
            DropColumn("dbo.EmployeeTimerEntries", "ClockInLatitude");
        }
    }
}
