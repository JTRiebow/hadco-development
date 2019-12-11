namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddRoleAuthActivityIndex : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.RoleAuthActivities", new[] { "RoleID" });
            CreateIndex("dbo.RoleAuthActivities", new[] { "RoleID", "AuthActivityID" }, unique: true, name: "UX_RoleID_AuthActivityID");
            AddForeignKey("dbo.RoleAuthActivities", "AuthActivityID", "dbo.AuthActivities", "AuthActivityID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RoleAuthActivities", "AuthActivityID", "dbo.AuthActivities");
            DropIndex("dbo.RoleAuthActivities", "UX_RoleID_AuthActivityID");
            CreateIndex("dbo.RoleAuthActivities", "RoleID");
        }
    }
}
