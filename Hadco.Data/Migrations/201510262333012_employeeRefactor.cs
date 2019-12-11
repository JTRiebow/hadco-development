namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class employeeRefactor : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Users", "EmployeeID", "dbo.Employees");
            DropForeignKey("dbo.UserRoles", "UserId", "dbo.Users");
            DropForeignKey("dbo.UserRoles", "RoleId", "dbo.Roles");
            DropIndex("dbo.Users", new[] { "EmployeeID" });
            DropIndex("dbo.Users", new[] { "Username" });
            DropIndex("dbo.UserRoles", new[] { "UserId" });
            DropIndex("dbo.UserRoles", new[] { "RoleId" });
            CreateTable(
                "dbo.EmployeeRoles",
                c => new
                    {
                        EmployeeID = c.Int(nullable: false),
                        RoleId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.EmployeeID, t.RoleId })
                .ForeignKey("dbo.Employees", t => t.EmployeeID, cascadeDelete: true)
                .ForeignKey("dbo.Roles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.EmployeeID)
                .Index(t => t.RoleId);
            
            AddColumn("dbo.Employees", "Username", c => c.String(nullable: false, maxLength: 64));
            AddColumn("dbo.Employees", "Password", c => c.String(nullable: false, maxLength: 256));
            AddColumn("dbo.Employees", "LastAuthenticatedOn", c => c.DateTimeOffset(precision: 7));
            CreateIndex("dbo.Employees", "Username", unique: true);
            DropTable("dbo.Users");
            DropTable("dbo.UserRoles");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.UserRoles",
                c => new
                    {
                        UserId = c.Int(nullable: false),
                        RoleId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId });
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        UserID = c.Int(nullable: false, identity: true),
                        EmployeeID = c.Int(),
                        Username = c.String(nullable: false, maxLength: 64),
                        Password = c.String(maxLength: 256),
                        UpdatePasswordToken = c.String(maxLength: 256),
                        UpdatePasswordTokenExpiration = c.DateTimeOffset(precision: 7),
                        CreatedOn = c.DateTimeOffset(precision: 7),
                        UpdatedOn = c.DateTimeOffset(precision: 7),
                        LastAuthenticatedOn = c.DateTimeOffset(precision: 7),
                    })
                .PrimaryKey(t => t.UserID);
            
            DropForeignKey("dbo.EmployeeRoles", "RoleId", "dbo.Roles");
            DropForeignKey("dbo.EmployeeRoles", "EmployeeID", "dbo.Employees");
            DropIndex("dbo.EmployeeRoles", new[] { "RoleId" });
            DropIndex("dbo.EmployeeRoles", new[] { "EmployeeID" });
            DropIndex("dbo.Employees", new[] { "Username" });
            DropColumn("dbo.Employees", "LastAuthenticatedOn");
            DropColumn("dbo.Employees", "Password");
            DropColumn("dbo.Employees", "Username");
            DropTable("dbo.EmployeeRoles");
            CreateIndex("dbo.UserRoles", "RoleId");
            CreateIndex("dbo.UserRoles", "UserId");
            CreateIndex("dbo.Users", "Username", unique: true);
            CreateIndex("dbo.Users", "EmployeeID");
            AddForeignKey("dbo.UserRoles", "RoleId", "dbo.Roles", "RoleID", cascadeDelete: true);
            AddForeignKey("dbo.UserRoles", "UserId", "dbo.Users", "UserID", cascadeDelete: true);
            AddForeignKey("dbo.Users", "EmployeeID", "dbo.Employees", "EmployeeID");
        }
    }
}
