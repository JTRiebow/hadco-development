namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MakeTimesheetEquipmentUseTimeNonNullable : DbMigration
    {
        public override void Up()
        {
            Sql(@"update dbo.Timesheets set EquipmentUseTime = 0 where EquipmentUseTime is null;");
            AlterColumn("dbo.Timesheets", "EquipmentUseTime", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Timesheets", "EquipmentUseTime", c => c.Int());
        }
    }
}
