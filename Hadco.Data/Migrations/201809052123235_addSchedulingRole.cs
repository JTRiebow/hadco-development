namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addSchedulingRole : DbMigration
    {
        public override void Up()
        {
            //Moved to Seed Method
            //Sql("insert into roles (Name) values ('Scheduling')");
        }
        
        public override void Down()
        {
            //Moved to Seed Method
            //Sql("delete from roles where Name = 'Scheduling'");
        }
    }
}
