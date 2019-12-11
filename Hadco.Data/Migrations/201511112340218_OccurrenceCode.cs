namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OccurrenceCode : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Occurrences", "Code", c => c.String(maxLength: 1));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Occurrences", "Code");
        }
    }
}
