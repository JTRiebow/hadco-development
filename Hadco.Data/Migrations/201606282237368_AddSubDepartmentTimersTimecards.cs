namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSubDepartmentTimersTimecards : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EmployeeTimers", "SubDepartmentID", c => c.Int(nullable: false));
            AddColumn("dbo.EmployeeTimecards", "SubDepartmentID", c => c.Int(nullable: false));
            Sql(@"update EmployeeTimers set SubDepartmentID = DepartmentID");
            Sql(@"update EmployeeTimecards set SubDepartmentID = DepartmentID");
        }
        
        public override void Down()
        {
            DropColumn("dbo.EmployeeTimecards", "SubDepartmentID");
            DropColumn("dbo.EmployeeTimers", "SubDepartmentID");
        }
    }
}
