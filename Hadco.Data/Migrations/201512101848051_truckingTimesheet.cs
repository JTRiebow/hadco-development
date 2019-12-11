namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class truckingTimesheet : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Timesheets", "PreTripStart", c => c.DateTimeOffset(precision: 7));
            AddColumn("dbo.Timesheets", "PreTripEnd", c => c.DateTimeOffset(precision: 7));
            AddColumn("dbo.Timesheets", "DepartureTime", c => c.DateTimeOffset(precision: 7));
            AddColumn("dbo.Timesheets", "ArrivalAtJob", c => c.DateTimeOffset(precision: 7));
            AddColumn("dbo.Timesheets", "PostJob", c => c.DateTimeOffset(precision: 7));
            AddColumn("dbo.Timesheets", "PostJobEnd", c => c.DateTimeOffset(precision: 7));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Timesheets", "PostJobEnd");
            DropColumn("dbo.Timesheets", "PostJob");
            DropColumn("dbo.Timesheets", "ArrivalAtJob");
            DropColumn("dbo.Timesheets", "DepartureTime");
            DropColumn("dbo.Timesheets", "PreTripEnd");
            DropColumn("dbo.Timesheets", "PreTripStart");
        }
    }
}
