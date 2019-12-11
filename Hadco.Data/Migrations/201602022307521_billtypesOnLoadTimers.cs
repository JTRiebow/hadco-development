namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class billtypesOnLoadTimers : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.LoadTimers", "BillTypeID", c => c.Int());
            CreateIndex("dbo.LoadTimers", "BillTypeID");
            AddForeignKey("dbo.LoadTimers", "BillTypeID", "dbo.BillTypes", "BillTypeID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.LoadTimers", "BillTypeID", "dbo.BillTypes");
            DropIndex("dbo.LoadTimers", new[] { "BillTypeID" });
            DropColumn("dbo.LoadTimers", "BillTypeID");
        }
    }
}
