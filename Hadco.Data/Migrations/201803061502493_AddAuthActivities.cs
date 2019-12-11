namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAuthActivities : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AuthActivities",
                c => new
                    {
                        AuthActivityID = c.Int(nullable: false),
                        AuthSectionID = c.Int(nullable: false),
                        Name = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.AuthActivityID)
                .ForeignKey("dbo.AuthSections", t => t.AuthSectionID, cascadeDelete: true)
                .Index(t => t.AuthSectionID);
            
            CreateTable(
                "dbo.AuthSections",
                c => new
                    {
                        AuthSectionID = c.Int(nullable: false),
                        Name = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.AuthSectionID);
            
            CreateTable(
                "dbo.RoleAuthActivities",
                c => new
                    {
                        RoleAuthActivityID = c.Int(nullable: false, identity: true),
                        RoleID = c.Int(nullable: false),
                        AuthActivityID = c.Int(nullable: false),
                        OwnDepartments = c.Boolean(nullable: false),
                        AllDepartments = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.RoleAuthActivityID)
                .ForeignKey("dbo.Roles", t => t.RoleID, cascadeDelete: true)
                .Index(t => t.RoleID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RoleAuthActivities", "RoleID", "dbo.Roles");
            DropForeignKey("dbo.AuthActivities", "AuthSectionID", "dbo.AuthSections");
            DropIndex("dbo.RoleAuthActivities", new[] { "RoleID" });
            DropIndex("dbo.AuthActivities", new[] { "AuthSectionID" });
            DropTable("dbo.RoleAuthActivities");
            DropTable("dbo.AuthSections");
            DropTable("dbo.AuthActivities");
        }
    }
}
