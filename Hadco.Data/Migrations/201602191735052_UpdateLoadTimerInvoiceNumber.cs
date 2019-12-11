namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateLoadTimerInvoiceNumber : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.LoadTimers", "InvoiceNumber");
            AddColumn("dbo.LoadTimers", "InvoiceNumber", c => c.String(maxLength: 32));
        }
        
        public override void Down()
        {
            DropColumn("dbo.LoadTimers", "InvoiceNumber");
            AddColumn("dbo.LoadTimers", "InvoiceNumber", c => c.Int(nullable: false));
        }
    }
}
