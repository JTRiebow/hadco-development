namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IncreaseInvoiceNumberLength : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.LoadTimers", "InvoiceNumber", c => c.String(maxLength: 50));
            AlterColumn("dbo.JobTimers", "InvoiceNumber", c => c.String(maxLength: 50));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.JobTimers", "InvoiceNumber", c => c.String(maxLength: 32));
            AlterColumn("dbo.LoadTimers", "InvoiceNumber", c => c.String(maxLength: 32));
        }
    }
}
