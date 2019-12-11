namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class rawSchema : DbMigration
    {
        public override void Up()
        {
            Sql("Create Schema raw");
        }
        
        public override void Down()
        {
            Sql("Drop Schema raw");
        }
    }
}
