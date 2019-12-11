namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addingPinToEmployee : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Employees", "Pin", c => c.String(maxLength: 8));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Employees", "Pin");
        }
    }
}
