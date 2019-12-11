namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class datefilednullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Jobs", "DateFiled", c => c.DateTime());
        }
        
        public override void Down()
        {
            //AlterColumn("dbo.Jobs", "DateFiled", c => c.DateTime(nullable: false));
        }
    }
}
