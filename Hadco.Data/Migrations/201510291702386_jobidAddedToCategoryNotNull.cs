namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class jobidAddedToCategoryNotNull : DbMigration
    {
        public override void Up()
        {
            Sql(@"update c 
                  set c.JobID = j.JobID
                  from Categories c 
                  join Jobs j on j.JobNumber = c.JobNumber");
            AlterColumn("dbo.Categories", "JobID", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Categories", "JobID", c => c.Int());
        }
    }
}
