namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class computereaseemployee : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Employees", "OriginComputerEase", c => c.Boolean(nullable: false, defaultValue: true));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Employees", "OriginComputerEase");
        }
    }
}
