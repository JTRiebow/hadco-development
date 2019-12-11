namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedAuthorizeNoteToEmployeeTimer : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EmployeeTimers", "AuthorizeNoteID", c => c.Int());
            CreateIndex("dbo.EmployeeTimers", "AuthorizeNoteID");
            AddForeignKey("dbo.EmployeeTimers", "AuthorizeNoteID", "dbo.Notes", "NoteID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.EmployeeTimers", "AuthorizeNoteID", "dbo.Notes");
            DropIndex("dbo.EmployeeTimers", new[] { "AuthorizeNoteID" });
            DropColumn("dbo.EmployeeTimers", "AuthorizeNoteID");
        }
    }
}
