namespace Hadco.Data.Migrations
{
    using Hadco.Common.Enums;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddNoteTypes : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.NoteTypes",
                c => new
                    {
                        NoteTypeID = c.Int(nullable: false),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.NoteTypeID);

            Sql($@"insert into NoteTypes (NoteTypeID, Name) values
                ({(int)NoteTypeName.ClockIn}, '{NoteTypeName.ClockIn.ToString()}'),
                ({(int)NoteTypeName.ClockOut}, '{NoteTypeName.ClockOut.ToString()}'),
                ({(int)NoteTypeName.Other}, '{NoteTypeName.Other.ToString()}')");

            AddColumn("dbo.EmployeeTimerEntries", "ClockInNoteID", c => c.Int());
            AddColumn("dbo.EmployeeTimerEntries", "ClockOutNoteID", c => c.Int());
            AddColumn("dbo.Notes", "EmployeeTimerEntryID", c => c.Int());
            AddColumn("dbo.Notes", "NoteTypeID", c => c.Int(nullable: false));
            DropColumn("dbo.Notes", "SystemGenerated");

            Sql($@"
insert into Notes (Description, EmployeeTimerID, EmployeeTimerEntryID, ModifiedTime, NoteTypeID)
select ClockInNote, EmployeeTimerID, EmployeeTimerEntryID, isnull(ClockIn, getdate()), {(int)NoteTypeName.ClockIn}
from EmployeeTimerEntries
where ClockInNote is not null and ClockInNote != ''
union
select ClockOutNote, EmployeeTimerID, EmployeeTimerEntryID, isnull(ClockOut, getdate()), {(int)NoteTypeName.ClockOut}
from EmployeeTimerEntries
where ClockOutNote is not null and ClockOutNote != ''
");

            CreateIndex("dbo.EmployeeTimerEntries", "ClockInNoteID");
            CreateIndex("dbo.EmployeeTimerEntries", "ClockOutNoteID");
            CreateIndex("dbo.Notes", "EmployeeTimerEntryID");
            CreateIndex("dbo.Notes", "NoteTypeID");
            AddForeignKey("dbo.Notes", "EmployeeTimerEntryID", "dbo.EmployeeTimerEntries", "EmployeeTimerEntryID");
            AddForeignKey("dbo.Notes", "NoteTypeID", "dbo.NoteTypes", "NoteTypeID", cascadeDelete: true);
            AddForeignKey("dbo.EmployeeTimerEntries", "ClockInNoteID", "dbo.Notes", "NoteID");
            AddForeignKey("dbo.EmployeeTimerEntries", "ClockOutNoteID", "dbo.Notes", "NoteID");
            DropColumn("dbo.EmployeeTimerEntries", "ClockInNote");
            DropColumn("dbo.EmployeeTimerEntries", "ClockOutNote");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Notes", "SystemGenerated", c => c.Boolean(nullable: false));
            AddColumn("dbo.EmployeeTimerEntries", "ClockOutNote", c => c.String(maxLength: 256));
            AddColumn("dbo.EmployeeTimerEntries", "ClockInNote", c => c.String(maxLength: 256));
            DropForeignKey("dbo.EmployeeTimerEntries", "ClockOutNoteID", "dbo.Notes");
            DropForeignKey("dbo.EmployeeTimerEntries", "ClockInNoteID", "dbo.Notes");
            DropForeignKey("dbo.Notes", "NoteTypeID", "dbo.NoteTypes");
            DropForeignKey("dbo.Notes", "EmployeeTimerEntryID", "dbo.EmployeeTimerEntries");
            DropIndex("dbo.Notes", new[] { "NoteTypeID" });
            DropIndex("dbo.Notes", new[] { "EmployeeTimerEntryID" });
            DropIndex("dbo.EmployeeTimerEntries", new[] { "ClockOutNoteID" });
            DropIndex("dbo.EmployeeTimerEntries", new[] { "ClockInNoteID" });
            DropColumn("dbo.Notes", "NoteTypeID");
            DropColumn("dbo.Notes", "EmployeeTimerEntryID");
            DropColumn("dbo.EmployeeTimerEntries", "ClockOutNoteID");
            DropColumn("dbo.EmployeeTimerEntries", "ClockInNoteID");
            DropTable("dbo.NoteTypes");
        }
    }
}
