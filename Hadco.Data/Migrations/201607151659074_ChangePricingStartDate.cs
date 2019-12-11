namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangePricingStartDate : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Pricings", name: "StartTime", newName: "StartDate");
            RenameColumn(table: "dbo.Pricings", name: "EndTime", newName: "EndDate");
        }
        
        public override void Down()
        {
            RenameColumn(table: "dbo.Pricings", name: "StartDate", newName: "StartTime");
            RenameColumn(table: "dbo.Pricings", name: "EndDate", newName: "EndTime");
        }
    }
}
