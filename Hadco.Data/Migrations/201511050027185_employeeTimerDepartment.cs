namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class employeeTimerDepartment : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EmployeeTimers", "DepartmentID", c => c.Int(nullable: false, defaultValue:1));
            CreateIndex("dbo.EmployeeTimers", "DepartmentID");
            AddForeignKey("dbo.EmployeeTimers", "DepartmentID", "dbo.Departments", "DepartmentID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.EmployeeTimers", "DepartmentID", "dbo.Departments");
            DropIndex("dbo.EmployeeTimers", new[] { "DepartmentID" });
            DropColumn("dbo.EmployeeTimers", "DepartmentID");
        }
    }
}
