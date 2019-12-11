namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addCustomerTypes : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CustomerTypes",
                c => new
                    {
                        CustomerTypeID = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 32),
                    })
                .PrimaryKey(t => t.CustomerTypeID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.CustomerTypes");
        }
    }
}
