namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class numberFieldIndexes : DbMigration
    {
        public override void Up()
        {
            Sql(@"create index idx_employeenumber on dbo.[Employees] (EmployeeNumber);
            create index idx_customernumber on dbo.[Customers](CustomerNumber);
            create index idx_equipmentnumber on dbo.[Equipment](EquipmentNumber);
            create index idx_jobnumber on dbo.[Jobs](JobNumber);
            create index idx_phasenumber on dbo.[Phases](PhaseNumber, JobNumber);
            create index idx_categorynumber on dbo.[Categories] (CategoryNumber, PhaseNumber, JobNumber);");
        }
        
        public override void Down()
        {
            Sql(@"drop index idx_employeenumber on dbo.[Employees];
                    drop index idx_customernumber on dbo.[Customers];
                    drop index idx_equipmentnumber on dbo.[Equipment];
                    drop index idx_jobnumber on dbo.[Jobs];
                    drop index idx_phasenumber on dbo.[Phases];
                    drop index idx_categorynumber on dbo.[Categories];");
        }
    }
}
