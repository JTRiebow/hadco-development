namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class userModelChanges : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Users", new[] { "EmailAddress" });
            AddColumn("dbo.Users", "Username", c => c.String(nullable: false, maxLength: 64));
            CreateIndex("dbo.Users", "Username", unique: true);
            DropColumn("dbo.Users", "EmailAddress");
            DropColumn("dbo.Users", "FirstName");
            DropColumn("dbo.Users", "LastName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Users", "LastName", c => c.String(maxLength: 64));
            AddColumn("dbo.Users", "FirstName", c => c.String(maxLength: 64));
            AddColumn("dbo.Users", "EmailAddress", c => c.String(nullable: false, maxLength: 64));
            DropIndex("dbo.Users", new[] { "Username" });
            DropColumn("dbo.Users", "Username");
            CreateIndex("dbo.Users", "EmailAddress", unique: true);
        }
    }
}
