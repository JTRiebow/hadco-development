namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class timerEntryHistory : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EmployeeTimerEntryHistories",
                c => new
                    {
                        EmployeeTimerEntryHistoryID = c.Int(nullable: false, identity: true),
                        EmployeeTimerEntryID = c.Int(nullable: false),
                        PreviousClockIn = c.DateTimeOffset(precision: 7),
                        PreviousClockOut = c.DateTimeOffset(precision: 7),
                        CurrentClockIn = c.DateTimeOffset(precision: 7),
                        CurrentClockOut = c.DateTimeOffset(precision: 7),
                        ChangedByID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.EmployeeTimerEntryHistoryID)
                .ForeignKey("dbo.Employees", t => t.ChangedByID, cascadeDelete: true)
                .ForeignKey("dbo.EmployeeTimerEntries", t => t.EmployeeTimerEntryID, cascadeDelete: true)
                .Index(t => t.EmployeeTimerEntryID)
                .Index(t => t.ChangedByID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.EmployeeTimerEntryHistories", "EmployeeTimerEntryID", "dbo.EmployeeTimerEntries");
            DropForeignKey("dbo.EmployeeTimerEntryHistories", "ChangedByID", "dbo.Employees");
            DropIndex("dbo.EmployeeTimerEntryHistories", new[] { "ChangedByID" });
            DropIndex("dbo.EmployeeTimerEntryHistories", new[] { "EmployeeTimerEntryID" });
            DropTable("dbo.EmployeeTimerEntryHistories");
        }
    }
}
