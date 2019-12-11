namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddEquipmentTimerEntries : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.EquipmentTimers", "EquipmentServiceTypeID", "dbo.EquipmentServiceTypes");
            DropIndex("dbo.EquipmentTimers", new[] { "EquipmentServiceTypeID" });
            CreateTable(
                "dbo.EquipmentTimerEntries",
                c => new
                    {
                        EquipmentTimerEntryID = c.Int(nullable: false, identity: true),
                        EquipmentTimerID = c.Int(nullable: false),
                        EquipmentServiceTypeID = c.Int(nullable: false),
                        StartTime = c.DateTimeOffset(precision: 7),
                        StopTime = c.DateTimeOffset(precision: 7),
                        Diary = c.String(),
                        Closed = c.Boolean(),
                    })
                .PrimaryKey(t => t.EquipmentTimerEntryID)
                .ForeignKey("dbo.EquipmentServiceTypes", t => t.EquipmentServiceTypeID, cascadeDelete: true)
                .ForeignKey("dbo.EquipmentTimers", t => t.EquipmentTimerID, cascadeDelete: true)
                .Index(t => t.EquipmentTimerID)
                .Index(t => t.EquipmentServiceTypeID);

            Sql(@"insert into dbo.EquipmentTimerEntries
                    (EquipmentTimerID, EquipmentServiceTypeID, StartTime, StopTime, Diary, Closed)
                select EquipmentTimerID, EquipmentServiceTypeID, StartTime, StopTime, Diary, Closed
                    from dbo.EquipmentTimers where StartTime is not null;");

            DropColumn("dbo.EquipmentTimers", "EquipmentServiceTypeID");
            DropColumn("dbo.EquipmentTimers", "StartTime");
            DropColumn("dbo.EquipmentTimers", "StopTime");
            DropColumn("dbo.EquipmentTimers", "Diary");
            DropColumn("dbo.EquipmentTimers", "Closed");
        }
        
        public override void Down()
        {
            AddColumn("dbo.EquipmentTimers", "Closed", c => c.Boolean());
            AddColumn("dbo.EquipmentTimers", "Diary", c => c.String());
            AddColumn("dbo.EquipmentTimers", "StopTime", c => c.DateTimeOffset(precision: 7));
            AddColumn("dbo.EquipmentTimers", "StartTime", c => c.DateTimeOffset(precision: 7));
            AddColumn("dbo.EquipmentTimers", "EquipmentServiceTypeID", c => c.Int());
            DropForeignKey("dbo.EquipmentTimerEntries", "EquipmentTimerID", "dbo.EquipmentTimers");
            DropForeignKey("dbo.EquipmentTimerEntries", "EquipmentServiceTypeID", "dbo.EquipmentServiceTypes");
            DropIndex("dbo.EquipmentTimerEntries", new[] { "EquipmentServiceTypeID" });
            DropIndex("dbo.EquipmentTimerEntries", new[] { "EquipmentTimerID" });

            Sql(@"update dbo.EquipmentTimers set EquipmentServiceTypeID = ete.EquipmentServiceTypeID, StartTime = ete.StartTime, StopTime = ete.StopTime, Diary = ete.Diary, Closed = ete.Closed             
                    from dbo.EquipmentTimerEntries ete where dbo.EquipmentTimers.EquipmentTimerID = ete.EquipmentTimerID;");

            DropTable("dbo.EquipmentTimerEntries");;
            CreateIndex("dbo.EquipmentTimers", "EquipmentServiceTypeID");
            AddForeignKey("dbo.EquipmentTimers", "EquipmentServiceTypeID", "dbo.EquipmentServiceTypes", "EquipmentServiceTypeID", cascadeDelete: true);

        }
    }
}