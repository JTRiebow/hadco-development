namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUpdatedTimeToJobTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Jobs", "ModifiedDate", c => c.DateTimeOffset(nullable: false, precision: 7));
            DropColumn("dbo.JobTimers", "PreviousQuantity");
            DropColumn("dbo.JobTimers", "OtherNewQuantity");
        }
        
        public override void Down()
        {
            AddColumn("dbo.JobTimers", "OtherNewQuantity", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.JobTimers", "PreviousQuantity", c => c.Decimal(precision: 18, scale: 2));
            DropColumn("dbo.Jobs", "ModifiedDate");
        }
    }
}
