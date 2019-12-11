namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddKeyToPitName : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Pits", "Name", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.Pits", "Name", unique: true);
        }
        
        public override void Down()
        {
            DropIndex("dbo.Pits", new[] { "Name" });
            AlterColumn("dbo.Pits", "Name", c => c.String(maxLength: 128));
        }
    }
}
