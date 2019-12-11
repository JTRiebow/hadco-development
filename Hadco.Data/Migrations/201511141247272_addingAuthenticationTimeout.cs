namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addingAuthenticationTimeout : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Departments", "AuthenticationTimeoutMinutes", c => c.Int(nullable: false, defaultValue:180));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Departments", "AuthenticationTimeoutMinutes");
        }
    }
}
