namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TimecardUpdate2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.EmployeeTimecard", "StartOfWeek", c => c.DateTime(nullable: false, storeType: "date"));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.EmployeeTimecard", "StartOfWeek", c => c.DateTime(nullable: false));
        }
    }
}
