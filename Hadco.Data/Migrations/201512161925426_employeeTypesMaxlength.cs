namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class employeeTypesMaxlength : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.EmployeeTypes", "Description", c => c.String(maxLength: 32));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.EmployeeTypes", "Description", c => c.String());
        }
    }
}
