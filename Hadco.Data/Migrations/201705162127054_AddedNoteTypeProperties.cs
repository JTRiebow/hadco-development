namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedNoteTypeProperties : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.NoteTypes", "Description", c => c.String());
            AddColumn("dbo.NoteTypes", "IsSystemGenerated", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.NoteTypes", "IsSystemGenerated");
            DropColumn("dbo.NoteTypes", "Description");
        }
    }
}
