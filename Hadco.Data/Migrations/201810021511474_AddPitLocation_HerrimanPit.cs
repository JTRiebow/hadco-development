namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPitLocation_HerrimanPit : DbMigration
    {
        public override void Up()
        {
            //Moved to Seed Method
            //Sql("insert into pits (Name) values ('Herriman Pit')");
        }
        
        public override void Down()
        {
            //Move to Seed Method
            //Sql("delete from pits where Name = 'Herriman Pit'");
        }
    }
}
