namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IdentityColumnChange : DbMigration
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
                    CREATE TABLE dbo.Tmp_Resources
                        (
                        ResourceID int NOT NULL,
                        Name nvarchar(128) NOT NULL
                        )  ON[PRIMARY]
                    GO
                    ALTER TABLE dbo.Tmp_Resources SET(LOCK_ESCALATION = TABLE)
                    GO
                    IF EXISTS(SELECT * FROM dbo.Resources)
                         EXEC('INSERT INTO dbo.Tmp_Resources (ResourceID, Name)
                            SELECT ResourceID, Name FROM dbo.Resources WITH(HOLDLOCK TABLOCKX)')
                    GO
                    ALTER TABLE dbo.RoleResources
                        DROP CONSTRAINT[FK_dbo.RoleResources_dbo.Resources_ResourceID]
                    GO
                    DROP TABLE dbo.Resources
                    GO
                    EXECUTE sp_rename N'dbo.Tmp_Resources', N'Resources', 'OBJECT'
                    GO
                    ALTER TABLE dbo.Resources ADD CONSTRAINT
                        [PK_dbo.Resources] PRIMARY KEY CLUSTERED
                        (
                        ResourceID
                        ) WITH(STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY]

                    GO
                    CREATE UNIQUE NONCLUSTERED INDEX IX_Name ON dbo.Resources
                        (
                        Name
                        ) WITH(STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY]
                    GO
                    COMMIT
                    BEGIN TRANSACTION
                    GO
                    ALTER TABLE dbo.RoleResources ADD CONSTRAINT
                        [FK_dbo.RoleResources_dbo.Resources_ResourceID] FOREIGN KEY
                        (
                        ResourceID
                        ) REFERENCES dbo.Resources
                        (
                        ResourceID
                        ) ON UPDATE  NO ACTION
                         ON DELETE  CASCADE

                    GO
                    ALTER TABLE dbo.RoleResources SET(LOCK_ESCALATION = TABLE)
                    GO
                    COMMIT");
            Sql(@"  delete Resources
                    where resourceid > 1000");
        }
        
        public override void Down()
        {
        }
    }
}
