namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class employeeType : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EmployeeTypes",
                c => new
                {
                    EmployeeTypeID = c.Int(nullable: false),
                    Description = c.String(),
                })
                .PrimaryKey(t => t.EmployeeTypeID);

            Sql("insert into dbo.EmployeeTypes values (1, 'Part Time');");
            Sql("insert into dbo.EmployeeTypes values (2, 'Full Time');");
            Sql("insert into dbo.EmployeeTypes values (3, 'Salaried');");

            AddColumn("dbo.Employees", "EmployeeTypeID", c => c.Int(nullable: false, defaultValue: 2));
            CreateIndex("dbo.Employees", "EmployeeTypeID");
            AddForeignKey("dbo.Employees", "EmployeeTypeID", "dbo.EmployeeTypes", "EmployeeTypeID", cascadeDelete: true);
        }

        public override void Down()
        {
            DropForeignKey("dbo.Employees", "EmployeeTypeID", "dbo.EmployeeTypes");
            DropIndex("dbo.Employees", new[] { "EmployeeTypeID" });
            DropColumn("dbo.Employees", "EmployeeTypeID");
            DropTable("dbo.EmployeeTypes");
        }
    }
}
