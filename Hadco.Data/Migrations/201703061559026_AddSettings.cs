namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSettings : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Settings",
                c => new
                    {
                        SettingID = c.Int(nullable: false, identity: true),
                        BreadCrumbSeconds = c.Int(nullable: false),
                        ModifiedTime = c.DateTimeOffset(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => t.SettingID);
            Sql("insert into Settings (BreadCrumbSeconds, ModifiedTime) values (600, getdate())");
        }
        
        public override void Down()
        {
            DropTable("dbo.Settings");
        }
    }
}
