namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class billtypes : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BillTypes",
                c => new
                    {
                        BillTypeID = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 32),
                    })
                .PrimaryKey(t => t.BillTypeID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.BillTypes");
        }
    }
}
