namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreation : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Resources",
                c => new
                    {
                        ResourceID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.ResourceID)
                .Index(t => t.Name, unique: true);
            
            CreateTable(
                "dbo.RoleResources",
                c => new
                    {
                        RoleID = c.Int(nullable: false),
                        ResourceID = c.Int(nullable: false),
                        Read = c.Boolean(nullable: false),
                        Write = c.Boolean(nullable: false),
                        Delete = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => new { t.RoleID, t.ResourceID })
                .ForeignKey("dbo.Resources", t => t.ResourceID, cascadeDelete: true)
                .ForeignKey("dbo.Roles", t => t.RoleID, cascadeDelete: true)
                .Index(t => t.RoleID)
                .Index(t => t.ResourceID);
            
            CreateTable(
                "dbo.Roles",
                c => new
                    {
                        RoleID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.RoleID)
                .Index(t => t.Name, unique: true);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        UserID = c.Int(nullable: false, identity: true),
                        EmailAddress = c.String(nullable: false, maxLength: 64),
                        FirstName = c.String(maxLength: 64),
                        LastName = c.String(maxLength: 64),
                        Password = c.String(maxLength: 256),
                        UpdatePasswordToken = c.String(maxLength: 256),
                        UpdatePasswordTokenExpiration = c.DateTimeOffset(precision: 7),
                        CreatedOn = c.DateTimeOffset(precision: 7),
                        UpdatedOn = c.DateTimeOffset(precision: 7),
                        LastAuthenticatedOn = c.DateTimeOffset(precision: 7),
                    })
                .PrimaryKey(t => t.UserID)
                .Index(t => t.EmailAddress, unique: true);
            
            CreateTable(
                "dbo.UserRoles",
                c => new
                    {
                        UserId = c.Int(nullable: false),
                        RoleId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.Roles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserRoles", "RoleId", "dbo.Roles");
            DropForeignKey("dbo.UserRoles", "UserId", "dbo.Users");
            DropForeignKey("dbo.RoleResources", "RoleID", "dbo.Roles");
            DropForeignKey("dbo.RoleResources", "ResourceID", "dbo.Resources");
            DropIndex("dbo.UserRoles", new[] { "RoleId" });
            DropIndex("dbo.UserRoles", new[] { "UserId" });
            DropIndex("dbo.Users", new[] { "EmailAddress" });
            DropIndex("dbo.Roles", new[] { "Name" });
            DropIndex("dbo.RoleResources", new[] { "ResourceID" });
            DropIndex("dbo.RoleResources", new[] { "RoleID" });
            DropIndex("dbo.Resources", new[] { "Name" });
            DropTable("dbo.UserRoles");
            DropTable("dbo.Users");
            DropTable("dbo.Roles");
            DropTable("dbo.RoleResources");
            DropTable("dbo.Resources");
        }
    }
}
