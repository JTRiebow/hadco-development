namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class nullableFieldJobTimer : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.JobTimers", "StartTime", c => c.DateTimeOffset(precision: 7));
            AlterColumn("dbo.JobTimers", "StopTime", c => c.DateTimeOffset(precision: 7));
        }
        
        public override void Down()
        {
            //AlterColumn("dbo.JobTimers", "StopTime", c => c.DateTimeOffset(nullable: false, precision: 7));
            //AlterColumn("dbo.JobTimers", "StartTime", c => c.DateTimeOffset(nullable: false, precision: 7));
        }
    }
}
