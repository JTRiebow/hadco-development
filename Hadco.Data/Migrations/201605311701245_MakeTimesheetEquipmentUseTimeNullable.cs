namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MakeTimesheetEquipmentUseTimeNullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Timesheets", "EquipmentUseTime", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Timesheets", "EquipmentUseTime", c => c.Int(nullable: false));
        }
    }
}
