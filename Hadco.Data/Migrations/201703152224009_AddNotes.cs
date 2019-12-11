namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddNotes : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Notes",
                c => new
                    {
                        NoteID = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                        EmployeeTimerID = c.Int(nullable: false),
                        SystemGenerated = c.Boolean(nullable: false),
                        Resolved = c.Boolean(nullable: false, defaultValue: false),
                        ResolvedTime = c.DateTimeOffset(precision: 7),
                        ResolvedEmployeeID = c.Int(),
                        ModifiedTime = c.DateTimeOffset(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => t.NoteID)
                .ForeignKey("dbo.EmployeeTimers", t => t.EmployeeTimerID, cascadeDelete: true)
                .ForeignKey("dbo.Employees", t => t.ResolvedEmployeeID)
                .Index(t => t.EmployeeTimerID)
                .Index(t => t.ResolvedEmployeeID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Notes", "ResolvedEmployeeID", "dbo.Employees");
            DropForeignKey("dbo.Notes", "EmployeeTimerID", "dbo.EmployeeTimers");
            DropIndex("dbo.Notes", new[] { "ResolvedEmployeeID" });
            DropIndex("dbo.Notes", new[] { "EmployeeTimerID" });
            DropTable("dbo.Notes");
        }
    }
}
