namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removeIdentityFromSeededEntities : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.LoadTimers", "BillTypeID", "dbo.BillTypes");
            DropForeignKey("dbo.Pricings", "BillTypeID", "dbo.BillTypes");
            DropForeignKey("dbo.EmployeeRoles", "RoleId", "dbo.Roles");
            DropForeignKey("dbo.RoleResources", "RoleID", "dbo.Roles");
            DropForeignKey("dbo.Pricings", "CustomerTypeID", "dbo.CustomerTypes");
            DropPrimaryKey("dbo.BillTypes");
            DropPrimaryKey("dbo.Roles");
            DropPrimaryKey("dbo.CustomerTypes");
            DropPrimaryKey("dbo.TruckClassifications");
            AlterColumn("dbo.BillTypes", "BillTypeID", c => c.Int(nullable: false));
            AlterColumn("dbo.Roles", "RoleID", c => c.Int(nullable: false));
            AlterColumn("dbo.CustomerTypes", "CustomerTypeID", c => c.Int(nullable: false));
            AlterColumn("dbo.TruckClassifications", "TruckClassificationID", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.BillTypes", "BillTypeID");
            AddPrimaryKey("dbo.Roles", "RoleID");
            AddPrimaryKey("dbo.CustomerTypes", "CustomerTypeID");
            AddPrimaryKey("dbo.TruckClassifications", "TruckClassificationID");
            AddForeignKey("dbo.LoadTimers", "BillTypeID", "dbo.BillTypes", "BillTypeID");
            AddForeignKey("dbo.Pricings", "BillTypeID", "dbo.BillTypes", "BillTypeID", cascadeDelete: true);
            AddForeignKey("dbo.EmployeeRoles", "RoleId", "dbo.Roles", "RoleID", cascadeDelete: true);
            AddForeignKey("dbo.RoleResources", "RoleID", "dbo.Roles", "RoleID", cascadeDelete: true);
            AddForeignKey("dbo.Pricings", "CustomerTypeID", "dbo.CustomerTypes", "CustomerTypeID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Pricings", "CustomerTypeID", "dbo.CustomerTypes");
            DropForeignKey("dbo.RoleResources", "RoleID", "dbo.Roles");
            DropForeignKey("dbo.EmployeeRoles", "RoleId", "dbo.Roles");
            DropForeignKey("dbo.Pricings", "BillTypeID", "dbo.BillTypes");
            DropForeignKey("dbo.LoadTimers", "BillTypeID", "dbo.BillTypes");
            DropPrimaryKey("dbo.TruckClassifications");
            DropPrimaryKey("dbo.CustomerTypes");
            DropPrimaryKey("dbo.Roles");
            DropPrimaryKey("dbo.BillTypes");
            AlterColumn("dbo.TruckClassifications", "TruckClassificationID", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.CustomerTypes", "CustomerTypeID", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.Roles", "RoleID", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.BillTypes", "BillTypeID", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.TruckClassifications", "TruckClassificationID");
            AddPrimaryKey("dbo.CustomerTypes", "CustomerTypeID");
            AddPrimaryKey("dbo.Roles", "RoleID");
            AddPrimaryKey("dbo.BillTypes", "BillTypeID");
            AddForeignKey("dbo.Pricings", "CustomerTypeID", "dbo.CustomerTypes", "CustomerTypeID", cascadeDelete: true);
            AddForeignKey("dbo.RoleResources", "RoleID", "dbo.Roles", "RoleID", cascadeDelete: true);
            AddForeignKey("dbo.EmployeeRoles", "RoleId", "dbo.Roles", "RoleID", cascadeDelete: true);
            AddForeignKey("dbo.Pricings", "BillTypeID", "dbo.BillTypes", "BillTypeID", cascadeDelete: true);
            AddForeignKey("dbo.LoadTimers", "BillTypeID", "dbo.BillTypes", "BillTypeID");
        }
    }
}
