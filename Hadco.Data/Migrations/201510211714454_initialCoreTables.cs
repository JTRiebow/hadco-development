namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initialCoreTables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Categories",
                c => new
                    {
                        CategoryID = c.Int(nullable: false, identity: true),
                        CategoryNumber = c.String(),
                        JobNumber = c.String(),
                        PhaseNumber = c.String(),
                        PhaseID = c.Int(nullable: false),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.CategoryID)
                .ForeignKey("dbo.Phases", t => t.PhaseID, cascadeDelete: true)
                .Index(t => t.PhaseID);
            
            CreateTable(
                "dbo.Phases",
                c => new
                    {
                        PhaseID = c.Int(nullable: false, identity: true),
                        PhaseNumber = c.String(),
                        JobID = c.Int(nullable: false),
                        JobNumber = c.String(),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.PhaseID)
                .ForeignKey("dbo.Jobs", t => t.JobID, cascadeDelete: true)
                .Index(t => t.JobID);
            
            CreateTable(
                "dbo.Jobs",
                c => new
                    {
                        JobID = c.Int(nullable: false, identity: true),
                        JobNumber = c.String(),
                        Name = c.String(),
                        Address1 = c.String(),
                        City = c.String(),
                        State = c.String(),
                        Zip = c.String(),
                        CustomerID = c.Int(nullable: false),
                        CustomerNumber = c.String(),
                        Class = c.String(),
                        DateOpen = c.DateTime(nullable: false),
                        Status = c.Int(nullable: false),
                        Memo = c.String(),
                        DateFiled = c.DateTime(nullable: false),
                        PreliminaryFilingNumber = c.String(),
                    })
                .PrimaryKey(t => t.JobID)
                .ForeignKey("dbo.Customers", t => t.CustomerID, cascadeDelete: true)
                .Index(t => t.CustomerID);
            
            CreateTable(
                "dbo.Customers",
                c => new
                    {
                        CustomerID = c.Int(nullable: false, identity: true),
                        CustomerNumber = c.String(),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.CustomerID);
            
            CreateTable(
                "dbo.Departments",
                c => new
                    {
                        DepartmentID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.DepartmentID);
            
            CreateTable(
                "dbo.Employees",
                c => new
                    {
                        EmployeeID = c.Int(nullable: false, identity: true),
                        EmployeeNumber = c.String(),
                        Name = c.String(),
                        Phone = c.String(),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.EmployeeID);
            
            CreateTable(
                "dbo.Equipment",
                c => new
                    {
                        EquipmentID = c.Int(nullable: false, identity: true),
                        EquipmentNumber = c.String(),
                        Name = c.String(),
                        Model = c.String(),
                        License = c.String(),
                        Fleetcode = c.String(),
                        SerialNumber = c.String(),
                        Mileage = c.Int(nullable: false),
                        HoursOfOperation = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Memo = c.String(),
                        Status = c.Int(nullable: false),
                        Type = c.String(),
                    })
                .PrimaryKey(t => t.EquipmentID);
            
            CreateTable(
                "dbo.EmployeeDepartments",
                c => new
                    {
                        EmployeeID = c.Int(nullable: false),
                        DepartmentID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.EmployeeID, t.DepartmentID })
                .ForeignKey("dbo.Employees", t => t.EmployeeID, cascadeDelete: true)
                .ForeignKey("dbo.Departments", t => t.DepartmentID, cascadeDelete: true)
                .Index(t => t.EmployeeID)
                .Index(t => t.DepartmentID);
            
            CreateTable(
                "dbo.EmployeeSupervisors",
                c => new
                    {
                        EmployeeID = c.Int(nullable: false),
                        SupervisorID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.EmployeeID, t.SupervisorID })
                .ForeignKey("dbo.Employees", t => t.EmployeeID)
                .ForeignKey("dbo.Employees", t => t.SupervisorID)
                .Index(t => t.EmployeeID)
                .Index(t => t.SupervisorID);
            
            AddColumn("dbo.Users", "EmployeeID", c => c.Int());
            CreateIndex("dbo.Users", "EmployeeID");
            AddForeignKey("dbo.Users", "EmployeeID", "dbo.Employees", "EmployeeID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Users", "EmployeeID", "dbo.Employees");
            DropForeignKey("dbo.EmployeeSupervisors", "SupervisorID", "dbo.Employees");
            DropForeignKey("dbo.EmployeeSupervisors", "EmployeeID", "dbo.Employees");
            DropForeignKey("dbo.EmployeeDepartments", "DepartmentID", "dbo.Departments");
            DropForeignKey("dbo.EmployeeDepartments", "EmployeeID", "dbo.Employees");
            DropForeignKey("dbo.Categories", "PhaseID", "dbo.Phases");
            DropForeignKey("dbo.Phases", "JobID", "dbo.Jobs");
            DropForeignKey("dbo.Jobs", "CustomerID", "dbo.Customers");
            DropIndex("dbo.EmployeeSupervisors", new[] { "SupervisorID" });
            DropIndex("dbo.EmployeeSupervisors", new[] { "EmployeeID" });
            DropIndex("dbo.EmployeeDepartments", new[] { "DepartmentID" });
            DropIndex("dbo.EmployeeDepartments", new[] { "EmployeeID" });
            DropIndex("dbo.Users", new[] { "EmployeeID" });
            DropIndex("dbo.Jobs", new[] { "CustomerID" });
            DropIndex("dbo.Phases", new[] { "JobID" });
            DropIndex("dbo.Categories", new[] { "PhaseID" });
            DropColumn("dbo.Users", "EmployeeID");
            DropTable("dbo.EmployeeSupervisors");
            DropTable("dbo.EmployeeDepartments");
            DropTable("dbo.Equipment");
            DropTable("dbo.Employees");
            DropTable("dbo.Departments");
            DropTable("dbo.Customers");
            DropTable("dbo.Jobs");
            DropTable("dbo.Phases");
            DropTable("dbo.Categories");
        }
    }
}
