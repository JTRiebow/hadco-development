namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class setJobCustomerIDNullable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Jobs", "CustomerID", "dbo.Customers");
            DropIndex("dbo.Jobs", new[] { "CustomerID" });
            AlterColumn("dbo.Jobs", "CustomerID", c => c.Int());
            CreateIndex("dbo.Jobs", "CustomerID");
            AddForeignKey("dbo.Jobs", "CustomerID", "dbo.Customers", "CustomerID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Jobs", "CustomerID", "dbo.Customers");
            DropIndex("dbo.Jobs", new[] { "CustomerID" });
            AlterColumn("dbo.Jobs", "CustomerID", c => c.Int(nullable: false));
            CreateIndex("dbo.Jobs", "CustomerID");
            AddForeignKey("dbo.Jobs", "CustomerID", "dbo.Customers", "CustomerID", cascadeDelete: true);
        }
    }
}
