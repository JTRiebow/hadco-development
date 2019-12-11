namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedTrackedByToDepartment : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Departments", "TrackedBy", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Departments", "TrackedBy");
        }
    }
}
