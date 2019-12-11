namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class timerEntryHistoryDateCreated : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EmployeeTimerEntryHistories", "ChangedTime", c => c.DateTimeOffset(nullable: false, precision: 7, defaultValue:DateTimeOffset.Now));
        }
        
        public override void Down()
        {
            DropColumn("dbo.EmployeeTimerEntryHistories", "ChangedTime");
        }
    }
}
