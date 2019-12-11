namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class identitychange : DbMigration
    {
        public override void Up()
        {
            //DropForeignKey("dbo.RoleResources", "ResourceID", "dbo.Resources");
            //DropPrimaryKey("dbo.Resources");
            //AlterColumn("dbo.Resources", "ResourceID", c => c.Int(nullable: false));
            //AddPrimaryKey("dbo.Resources", "ResourceID");
            //AddForeignKey("dbo.RoleResources", "ResourceID", "dbo.Resources", "ResourceID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            //DropForeignKey("dbo.RoleResources", "ResourceID", "dbo.Resources");
            //DropPrimaryKey("dbo.Resources");
            //AlterColumn("dbo.Resources", "ResourceID", c => c.Int(nullable: false, identity: true));
            //AddPrimaryKey("dbo.Resources", "ResourceID");
            //AddForeignKey("dbo.RoleResources", "ResourceID", "dbo.Resources", "ResourceID", cascadeDelete: true);
        }
    }
}
