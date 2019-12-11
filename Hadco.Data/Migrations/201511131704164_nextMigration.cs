namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class nextMigration : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.EmployeeTimecard", newName: "EmployeeTimecards");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.EmployeeTimecards", newName: "EmployeeTimecard");
        }
    }
}
