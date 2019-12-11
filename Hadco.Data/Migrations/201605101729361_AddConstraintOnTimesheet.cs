namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddConstraintOnTimesheet : DbMigration
    {
        public override void Up()
        {
            //DropIndex("dbo.Timesheets", new[] { "DepartmentID" });
            //DropIndex("dbo.Timesheets", new[] { "EmployeeID" });
            //CreateIndex("dbo.Timesheets", new[] { "DepartmentID", "EmployeeID", "Day" }, unique: true, name: "IX_DepartmentIDEmployeeIDAndDay");
        }
        
        public override void Down()
        {
            //DropIndex("dbo.Timesheets", "IX_DepartmentIDEmployeeIDAndDay");
            //CreateIndex("dbo.Timesheets", "EmployeeID");
            //CreateIndex("dbo.Timesheets", "DepartmentID");
        }
    }
}
