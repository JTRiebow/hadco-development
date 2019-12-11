namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ConfigureClockInStatus : DbMigration
    {
        public override void Up()
        {
            //CreateTable(
            //    "dbo.ClockedInStatus",
            //    c => new
            //        {
            //            EmployeeID = c.Int(nullable: false),
            //            IsClockedIn = c.Boolean(nullable: false),
            //        })
            //    .PrimaryKey(t => t.EmployeeID)
            //    .ForeignKey("dbo.Employees", t => t.EmployeeID)
            //    .Index(t => t.EmployeeID);
            
        }
        
        public override void Down()
        {
            //DropForeignKey("dbo.ClockedInStatus", "EmployeeID", "dbo.Employees");
            //DropIndex("dbo.ClockedInStatus", new[] { "EmployeeID" });
            //DropTable("dbo.ClockedInStatus");
        }
    }
}
