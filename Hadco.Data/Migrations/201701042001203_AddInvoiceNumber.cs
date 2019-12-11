namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddInvoiceNumber : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.JobTimers", "InvoiceNumber", c => c.String(maxLength: 32));
        }
        
        public override void Down()
        {
            DropColumn("dbo.JobTimers", "InvoiceNumber");
        }
    }
}
