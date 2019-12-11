namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addTruckClassifications : DbMigration
    {
        public override void Up()
        {  
            CreateTable(
                "dbo.TruckClassifications",
                c => new
                    {
                        TruckClassificationID = c.Int(nullable: false, identity: true),
                        Truck = c.String(nullable: false, maxLength: 32),
                        Trailer1 = c.String(maxLength: 8),
                        Trailer2 = c.String(maxLength: 8),
                    })
                .PrimaryKey(t => t.TruckClassificationID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.TruckClassifications");
        }
    }
}
