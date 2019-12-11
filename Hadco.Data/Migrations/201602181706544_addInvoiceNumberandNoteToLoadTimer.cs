namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addInvoiceNumberandNoteToLoadTimer : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.LoadTimers", "InvoiceNumber", c => c.Int(nullable: false));
            AddColumn("dbo.LoadTimers", "Note", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.LoadTimers", "Note");
            DropColumn("dbo.LoadTimers", "InvoiceNumber");
        }
    }
}
