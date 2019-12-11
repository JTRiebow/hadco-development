namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveTruckerDailyView : DbMigration
    {
        public override void Up()
        {
            //DropTable("dbo.TruckerDailies");
        }
        
        public override void Down()
        {
            //CreateTable(
            //    "dbo.TruckerDailies",
            //    c => new
            //        {
            //            TruckerDailyID = c.Int(nullable: false, identity: true),
            //            LoadTimerID = c.Int(nullable: false),
            //            DowntimeTimerID = c.Int(),
            //            BillType = c.String(),
            //            Date = c.DateTime(nullable: false),
            //            Name = c.String(),
            //            Truck = c.String(),
            //            Trailer = c.String(),
            //            PupTrailer = c.String(),
            //            TicketNumber = c.String(),
            //            DowntimeCode = c.String(),
            //            Tons = c.String(),
            //            LoadSite = c.String(),
            //            DumpSite = c.String(),
            //            Material = c.String(),
            //            LoadEquipment = c.String(),
            //            Job = c.String(),
            //            JobID = c.Int(),
            //            Phase = c.String(),
            //            PhaseID = c.Int(),
            //            Category = c.String(),
            //            CategoryID = c.Int(),
            //            TotalHours = c.Decimal(precision: 18, scale: 2),
            //            InvoiceNumber = c.String(),
            //            Note = c.String(),
            //            Location = c.String(),
            //            DepartmentID = c.Int(nullable: false),
            //            PricePerUnit = c.Decimal(precision: 18, scale: 2),
            //            CalculatedRevenue = c.Decimal(precision: 18, scale: 2),
            //        })
            //    .PrimaryKey(t => t.TruckerDailyID);
            
        }
    }
}
