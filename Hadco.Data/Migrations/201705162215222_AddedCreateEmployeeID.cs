namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedCreateEmployeeID : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Notes", "CreatedTime", c => c.DateTimeOffset(precision: 7));
            AddColumn("dbo.Notes", "CreatedEmployeeID", c => c.Int());
            CreateIndex("dbo.Notes", "CreatedEmployeeID");
            AddForeignKey("dbo.Notes", "CreatedEmployeeID", "dbo.Employees", "EmployeeID");
            Sql(@"
if not exists (select * from Employees where Username = 'system')
    insert into Employees (Name, Username, Password, EmployeeTypeID, OriginComputerEase, Status)
    values ('System', 'system', 'none', 3, 0, 2)");
            Sql(@"
declare @systemEmployeeId int = (select top 1 EmployeeID from Employees where Username = 'system');
update Notes set CreatedEmployeeID = @systemEmployeeId, CreatedTime = ModifiedTime where NoteTypeID in (3,4,6,7)
update Notes set CreatedEmployeeID = EmployeeID, CreatedTime = ModifiedTime where NoteTypeID in (1,2,5)");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Notes", "CreatedEmployeeID", "dbo.Employees");
            DropIndex("dbo.Notes", new[] { "CreatedEmployeeID" });
            DropColumn("dbo.Notes", "CreatedEmployeeID");
            DropColumn("dbo.Notes", "CreatedTime");
        }
    }
}
