namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removeDepartmentIdentity : DbMigration
    {
        public override void Up()
        {
Sql(@"BEGIN TRANSACTION
	SET QUOTED_IDENTIFIER ON
	SET ARITHABORT ON
	SET NUMERIC_ROUNDABORT OFF
	SET CONCAT_NULL_YIELDS_NULL ON
	SET ANSI_NULLS ON
	SET ANSI_PADDING ON
	SET ANSI_WARNINGS ON
COMMIT
                    
BEGIN TRANSACTION
	 GO
	 CREATE TABLE dbo.Tmp_Departments
	     (
	     DepartmentID int NOT NULL,
	     Name nvarchar(128) NOT NULL,
		AuthenticationTimeoutMinutes int NOT NULL
	     )  ON[PRIMARY]
	 GO
	 ALTER TABLE dbo.Tmp_Departments SET(LOCK_ESCALATION = TABLE)
	 GO
	 IF EXISTS(SELECT * FROM dbo.Departments)
	      EXEC('INSERT INTO dbo.Tmp_Departments (DepartmentID, Name, AuthenticationTimeoutMinutes)
	         SELECT DepartmentID, Name, AuthenticationTimeoutMinutes FROM dbo.Departments --WITH(HOLDLOCK TABLOCKX)')
	 GO
	ALTER TABLE dbo.EmployeeTimecards
	     DROP CONSTRAINT[FK_dbo.EmployeeTimecard_dbo.Departments_DepartmentID]
	ALTER TABLE dbo.Timesheets
		DROP CONSTRAINT[FK_dbo.Timesheets_dbo.Departments_DepartmentID]
	ALTER TABLE dbo.EmployeeTimers
		DROP CONSTRAINT[FK_dbo.EmployeeTimers_dbo.Departments_DepartmentID]
	ALTER TABLE dbo.EmployeeDepartments
		DROP CONSTRAINT[FK_dbo.EmployeeDepartments_dbo.Departments_DepartmentID]
	 GO
	 DROP TABLE dbo.Departments
	 GO
	 EXECUTE sp_rename N'Tmp_Departments', N'Departments', 'OBJECT'
	 GO
	 ALTER TABLE dbo.Departments ADD CONSTRAINT
	     [PK_dbo.Departments] PRIMARY KEY CLUSTERED
	     (
	     DepartmentID
	     ) WITH(STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY]
		
COMMIT

BEGIN TRANSACTION
  GO
ALTER TABLE dbo.EmployeeTimecards ADD CONSTRAINT
[FK_dbo.EmployeeTimecard_dbo.Departments_DepartmentID] FOREIGN KEY
(
DepartmentID
) REFERENCES dbo.Departments
(
DepartmentID
) ON UPDATE  NO ACTION
 ON DELETE  NO ACTION


ALTER TABLE dbo.Timesheets ADD CONSTRAINT
[FK_dbo.Timesheets_dbo.Departments_DepartmentID] FOREIGN KEY
(
DepartmentID
) REFERENCES dbo.Departments
(
DepartmentID
) ON UPDATE  NO ACTION
 ON DELETE  NO ACTION
                ALTER TABLE dbo.EmployeeTimers ADD CONSTRAINT
[FK_dbo.EmployeeTimers_dbo.Departments_DepartmentID] FOREIGN KEY
(
DepartmentID
) REFERENCES dbo.Departments
(
DepartmentID
) ON UPDATE  NO ACTION
 ON DELETE  NO ACTION

ALTER TABLE dbo.EmployeeDepartments ADD CONSTRAINT
[FK_dbo.EmployeeDepartments_dbo.Departments_DepartmentID] FOREIGN KEY
(
DepartmentID
) REFERENCES dbo.Departments
(
DepartmentID
) ON UPDATE  NO ACTION
 ON DELETE  CASCADE

GO
ALTER TABLE dbo.EmployeeTimecards SET(LOCK_ESCALATION = TABLE)
ALTER TABLE dbo.Timesheets SET(LOCK_ESCALATION = TABLE)
ALTER TABLE dbo.EmployeeTimers SET(LOCK_ESCALATION = TABLE)
ALTER TABLE dbo.EmployeeDepartments SET(LOCK_ESCALATION = TABLE)
 GO
COMMIT");


        }
        
        public override void Down()
        {
 
        }
    }
}
