namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedmaxlengthtofields : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Categories", "CategoryNumber", c => c.String(maxLength: 128));
            AlterColumn("dbo.Categories", "JobNumber", c => c.String(maxLength: 128));
            AlterColumn("dbo.Categories", "PhaseNumber", c => c.String(maxLength: 128));
            AlterColumn("dbo.Categories", "Name", c => c.String(maxLength: 128));
            AlterColumn("dbo.Phases", "PhaseNumber", c => c.String(maxLength: 128));
            AlterColumn("dbo.Phases", "JobNumber", c => c.String(maxLength: 128));
            AlterColumn("dbo.Phases", "Name", c => c.String(maxLength: 128));
            AlterColumn("dbo.Jobs", "JobNumber", c => c.String(maxLength: 128));
            AlterColumn("dbo.Jobs", "Name", c => c.String(maxLength: 128));
            AlterColumn("dbo.Jobs", "Address1", c => c.String(maxLength: 128));
            AlterColumn("dbo.Jobs", "City", c => c.String(maxLength: 128));
            AlterColumn("dbo.Jobs", "State", c => c.String(maxLength: 128));
            AlterColumn("dbo.Jobs", "Zip", c => c.String(maxLength: 128));
            AlterColumn("dbo.Jobs", "CustomerNumber", c => c.String(maxLength: 128));
            AlterColumn("dbo.Jobs", "Class", c => c.String(maxLength: 128));
            AlterColumn("dbo.Jobs", "PreliminaryFilingNumber", c => c.String(maxLength: 128));
            AlterColumn("dbo.Customers", "CustomerNumber", c => c.String(maxLength: 128));
            AlterColumn("dbo.Customers", "Name", c => c.String(maxLength: 128));
            AlterColumn("dbo.Departments", "Name", c => c.String(maxLength: 128));
            AlterColumn("dbo.Employees", "EmployeeNumber", c => c.String(maxLength: 128));
            AlterColumn("dbo.Employees", "Name", c => c.String(maxLength: 128));
            AlterColumn("dbo.Employees", "Phone", c => c.String(maxLength: 128));
            AlterColumn("dbo.Equipment", "EquipmentNumber", c => c.String(maxLength: 128));
            AlterColumn("dbo.Equipment", "Name", c => c.String(maxLength: 128));
            AlterColumn("dbo.Equipment", "Model", c => c.String(maxLength: 128));
            AlterColumn("dbo.Equipment", "License", c => c.String(maxLength: 128));
            AlterColumn("dbo.Equipment", "Fleetcode", c => c.String(maxLength: 128));
            AlterColumn("dbo.Equipment", "SerialNumber", c => c.String(maxLength: 128));
        }
        
        public override void Down()
        {
            // no need to go backwards here. This was just an oversight.

            //AlterColumn("dbo.Equipment", "SerialNumber", c => c.String());
            //AlterColumn("dbo.Equipment", "Fleetcode", c => c.String());
            //AlterColumn("dbo.Equipment", "License", c => c.String());
            //AlterColumn("dbo.Equipment", "Model", c => c.String());
            //AlterColumn("dbo.Equipment", "Name", c => c.String());
            //AlterColumn("dbo.Equipment", "EquipmentNumber", c => c.String());
            //AlterColumn("dbo.Employees", "Phone", c => c.String());
            //AlterColumn("dbo.Employees", "Name", c => c.String());
            //AlterColumn("dbo.Employees", "EmployeeNumber", c => c.String());
            //AlterColumn("dbo.Departments", "Name", c => c.String());
            //AlterColumn("dbo.Customers", "Name", c => c.String());
            //AlterColumn("dbo.Customers", "CustomerNumber", c => c.String());
            //AlterColumn("dbo.Jobs", "PreliminaryFilingNumber", c => c.String());
            //AlterColumn("dbo.Jobs", "Class", c => c.String());
            //AlterColumn("dbo.Jobs", "CustomerNumber", c => c.String());
            //AlterColumn("dbo.Jobs", "Zip", c => c.String());
            //AlterColumn("dbo.Jobs", "State", c => c.String());
            //AlterColumn("dbo.Jobs", "City", c => c.String());
            //AlterColumn("dbo.Jobs", "Address1", c => c.String());
            //AlterColumn("dbo.Jobs", "Name", c => c.String());
            //AlterColumn("dbo.Jobs", "JobNumber", c => c.String());
            //AlterColumn("dbo.Phases", "Name", c => c.String());
            //AlterColumn("dbo.Phases", "JobNumber", c => c.String());
            //AlterColumn("dbo.Phases", "PhaseNumber", c => c.String());
            //AlterColumn("dbo.Categories", "Name", c => c.String());
            //AlterColumn("dbo.Categories", "PhaseNumber", c => c.String());
            //AlterColumn("dbo.Categories", "JobNumber", c => c.String());
            //AlterColumn("dbo.Categories", "CategoryNumber", c => c.String());
        }
    }
}
