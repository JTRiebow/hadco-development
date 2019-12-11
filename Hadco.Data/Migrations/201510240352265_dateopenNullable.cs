namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dateopenNullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Jobs", "DateOpen", c => c.DateTime());
        }
        
        public override void Down()
        {
            //AlterColumn("dbo.Jobs", "DateOpen", c => c.DateTime(nullable: false));
        }
    }
}
