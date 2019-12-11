namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedCodeToEquipmentServiceType : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EquipmentServiceTypes", "Code", c => c.String(maxLength: 4));
        }
        
        public override void Down()
        {
            DropColumn("dbo.EquipmentServiceTypes", "Code");
        }
    }
}
