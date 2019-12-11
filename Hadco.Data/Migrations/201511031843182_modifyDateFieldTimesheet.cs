namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class modifyDateFieldTimesheet : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Timesheets", "Day", c => c.DateTime(nullable: false, storeType: "date"));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Timesheets", "Day", c => c.DateTime(nullable: false));
        }
    }
}
