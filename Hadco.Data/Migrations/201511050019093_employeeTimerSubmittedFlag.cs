namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class employeeTimerSubmittedFlag : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EmployeeTimers", "Submitted", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.EmployeeTimers", "Submitted");
        }
    }
}
