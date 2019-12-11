namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EmployeeTimecards : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EmployeeTimecard",
                c => new
                    {
                        EmployeeTimecardID = c.Int(nullable: false, identity: true),
                        EmployeeID = c.Int(nullable: false),
                        DepartmentID = c.Int(nullable: false),
                        StartOfWeek = c.DateTime(nullable: false),
                        ApprovedBySupervisor = c.Boolean(nullable: false),
                        ApprovedByAccounting = c.Boolean(nullable: false),
                        Flagged = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.EmployeeTimecardID)
                .ForeignKey("dbo.Departments", t => t.DepartmentID, cascadeDelete: true)
                .ForeignKey("dbo.Employees", t => t.EmployeeID, cascadeDelete: true)
                .Index(t => t.EmployeeID)
                .Index(t => t.DepartmentID);
            
            AddColumn("dbo.EmployeeTimers", "EmployeeTimecardID", c => c.Int());
            CreateIndex("dbo.EmployeeTimers", "EmployeeTimecardID");
            AddForeignKey("dbo.EmployeeTimers", "EmployeeTimecardID", "dbo.EmployeeTimecard", "EmployeeTimecardID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.EmployeeTimers", "EmployeeTimecardID", "dbo.EmployeeTimecard");
            DropForeignKey("dbo.EmployeeTimecard", "EmployeeID", "dbo.Employees");
            DropForeignKey("dbo.EmployeeTimecard", "DepartmentID", "dbo.Departments");
            DropIndex("dbo.EmployeeTimecard", new[] { "DepartmentID" });
            DropIndex("dbo.EmployeeTimecard", new[] { "EmployeeID" });
            DropIndex("dbo.EmployeeTimers", new[] { "EmployeeTimecardID" });
            DropColumn("dbo.EmployeeTimers", "EmployeeTimecardID");
            DropTable("dbo.EmployeeTimecard");
        }
    }
}
