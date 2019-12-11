namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addPitsAndAddPitIDToEmployeeTimerEntry : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Pits",
                c => new
                    {
                        PitID = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.PitID);
            
            AddColumn("dbo.EmployeeTimerEntries", "PitID", c => c.Int());
            CreateIndex("dbo.EmployeeTimerEntries", "PitID");
            AddForeignKey("dbo.EmployeeTimerEntries", "PitID", "dbo.Pits", "PitID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.EmployeeTimerEntries", "PitID", "dbo.Pits");
            DropIndex("dbo.EmployeeTimerEntries", new[] { "PitID" });
            DropColumn("dbo.EmployeeTimerEntries", "PitID");
            DropTable("dbo.Pits");
        }
    }
}
