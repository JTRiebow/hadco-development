namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddRoleAuthActivityDepartmentsTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RoleAuthActivityDepartments",
                c => new
                    {
                        RoleAuthActivityID = c.Int(nullable: false),
                        DepartmentID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.RoleAuthActivityID, t.DepartmentID })
                .ForeignKey("dbo.RoleAuthActivities", t => t.RoleAuthActivityID, cascadeDelete: true)
                .ForeignKey("dbo.Departments", t => t.DepartmentID, cascadeDelete: true)
                .Index(t => t.RoleAuthActivityID)
                .Index(t => t.DepartmentID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RoleAuthActivityDepartments", "DepartmentID", "dbo.Departments");
            DropForeignKey("dbo.RoleAuthActivityDepartments", "RoleAuthActivityID", "dbo.RoleAuthActivities");
            DropIndex("dbo.RoleAuthActivityDepartments", new[] { "DepartmentID" });
            DropIndex("dbo.RoleAuthActivityDepartments", new[] { "RoleAuthActivityID" });
            DropTable("dbo.RoleAuthActivityDepartments");
        }
    }
}
