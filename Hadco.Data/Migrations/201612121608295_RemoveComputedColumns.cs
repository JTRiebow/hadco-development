namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveComputedColumns : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.JobTimers", "PreviousQuantity");
            DropColumn("dbo.JobTimers", "OtherNewQuantity");
        }
        
        public override void Down()
        {
            AddColumn("dbo.JobTimers", "OtherNewQuantity", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.JobTimers", "PreviousQuantity", c => c.Decimal(precision: 18, scale: 2));
        }
    }
}
