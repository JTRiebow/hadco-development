namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangedRunIDtoPhaseID : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Pricings", name: "RunID", newName: "PhaseID");
            RenameIndex(table: "dbo.Pricings", name: "IX_RunID", newName: "IX_PhaseID");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.Pricings", name: "IX_PhaseID", newName: "IX_RunID");
            RenameColumn(table: "dbo.Pricings", name: "PhaseID", newName: "RunID");
        }
    }
}
