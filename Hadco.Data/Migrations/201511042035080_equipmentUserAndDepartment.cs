namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class equipmentUserAndDepartment : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Timesheets", "DepartmentID", c => c.Int(nullable: false, defaultValue:1));
            AddColumn("dbo.EmployeeTimers", "EquipmentUseTime", c => c.Int(nullable: false));
            CreateIndex("dbo.Timesheets", "DepartmentID");
            AddForeignKey("dbo.Timesheets", "DepartmentID", "dbo.Departments", "DepartmentID", cascadeDelete: false);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Timesheets", "DepartmentID", "dbo.Departments");
            DropIndex("dbo.Timesheets", new[] { "DepartmentID" });
            DropColumn("dbo.EmployeeTimers", "EquipmentUseTime");
            DropColumn("dbo.Timesheets", "DepartmentID");
        }
    }
}
