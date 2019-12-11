namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OverheadCodes : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.OverheadCodes",
                c => new
                    {
                        DepartmentID = c.Int(nullable: false),
                        Type = c.String(nullable: false, maxLength: 128),
                        JobNumber = c.String(),
                        PhaseNumber = c.String(),
                        CategoryNumber = c.String(),
                    })
                .PrimaryKey(t => new { t.DepartmentID, t.Type })
                .ForeignKey("dbo.Departments", t => t.DepartmentID, cascadeDelete: true)
                .Index(t => t.DepartmentID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OverheadCodes", "DepartmentID", "dbo.Departments");
            DropIndex("dbo.OverheadCodes", new[] { "DepartmentID" });
            DropTable("dbo.OverheadCodes");
        }
    }
}
