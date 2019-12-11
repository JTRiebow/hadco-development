using System.Diagnostics;

namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IncreaseNoteLength : DbMigration
    {
        public override void Up()
        {
            //AlterColumn("dbo.EmployeeTimerEntries", "ClockInNote", c => c.String());
            //AlterColumn("dbo.EmployeeTimerEntries", "ClockOutNote", c => c.String());

            Sql(@"
  IF EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'ClockInNote'
          AND Object_ID = Object_ID(N'dbo.EmployeeTimerEntries'))
		  alter table dbo.EmployeeTimerEntries alter column ClockInNote nvarchar(max)

IF EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'ClockOutNote'
          AND Object_ID = Object_ID(N'dbo.EmployeeTimerEntries'))
		  alter table dbo.EmployeeTimerEntries alter column ClockOutNote nvarchar(max)");
        }
        
        public override void Down()
        {
            //AlterColumn("dbo.EmployeeTimerEntries", "ClockOutNote", c => c.String(maxLength: 256));
            //AlterColumn("dbo.EmployeeTimerEntries", "ClockInNote", c => c.String(maxLength: 256));

            Sql(@"

  IF EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'ClockInNote'
          AND Object_ID = Object_ID(N'dbo.EmployeeTimerEntries'))
		  alter table dbo.EmployeeTimerEntries alter column ClockInNote nvarchar(256)

IF EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'ClockOutNote'
          AND Object_ID = Object_ID(N'dbo.EmployeeTimerEntries'))
		  alter table dbo.EmployeeTimerEntries alter column ClockOutNote nvarchar(256)");
        }
    }
}
