namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IdentityBugFix : DbMigration
    {
        public override void Up()
        {
            Sql(@"
                ALTER TABLE [dbo].[LoadTimers] DROP CONSTRAINT [FK_dbo.LoadTimers_dbo.BillTypes_BillTypeID];
                ALTER TABLE [dbo].[Pricings] DROP CONSTRAINT [FK_dbo.Pricings_dbo.BillTypes_BillTypeID];
                ALTER TABLE [dbo].[Pricings] DROP CONSTRAINT [FK_dbo.Pricings_dbo.CustomerTypes_CustomerTypeID];
                ALTER TABLE [dbo].[Jobs] DROP CONSTRAINT [FK_dbo.Jobs_dbo.CustomerTypes_CustomerTypeID];
                
                ALTER TABLE [dbo].[BillTypes] DROP CONSTRAINT [PK_dbo.BillTypes];
                ALTER TABLE [dbo].[CustomerTypes] DROP CONSTRAINT [PK_dbo.CustomerTypes];
                ALTER TABLE [dbo].[TruckClassifications] DROP CONSTRAINT [PK_dbo.TruckClassifications];
            ");

            AddColumn("dbo.BillTypes", "BillTypeID2", c => c.Int());
            Sql(@"update dbo.BillTypes set BillTypeID2 = BillTypeID;");
            AlterColumn("dbo.BillTypes", "BillTypeID2", c => c.Int(nullable: false));
            DropColumn("dbo.BillTypes", "BillTypeID");
            RenameColumn("dbo.BillTypes", "BillTypeID2", "BillTypeID");

            AddColumn("dbo.CustomerTypes", "CustomerTypeID2", c => c.Int());
            Sql(@"update dbo.CustomerTypes set CustomerTypeID2 = CustomerTypeID;");
            AlterColumn("dbo.CustomerTypes", "CustomerTypeID2", c => c.Int(nullable: false));
            DropColumn("dbo.CustomerTypes", "CustomerTypeID");
            RenameColumn("dbo.CustomerTypes", "CustomerTypeID2", "CustomerTypeID");

            AddColumn("dbo.TruckClassifications", "TruckClassificationID2", c => c.Int());
            Sql(@"update dbo.TruckClassifications set TruckClassificationID2 = TruckClassificationID;");
            AlterColumn("dbo.TruckClassifications", "TruckClassificationID2", c => c.Int(nullable: false));
            DropColumn("dbo.TruckClassifications", "TruckClassificationID");
            RenameColumn("dbo.TruckClassifications", "TruckClassificationID2", "TruckClassificationID");

            Sql(@"
                ALTER TABLE [dbo].[BillTypes] ADD  CONSTRAINT [PK_dbo.BillTypes] PRIMARY KEY CLUSTERED 
                (
                	[BillTypeID] ASC
                )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];

                ALTER TABLE [dbo].[CustomerTypes] ADD  CONSTRAINT [PK_dbo.CustomerTypes] PRIMARY KEY CLUSTERED 
                (
                	[CustomerTypeID] ASC
                )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];

                ALTER TABLE [dbo].[TruckClassifications] ADD  CONSTRAINT [PK_dbo.TruckClassifications] PRIMARY KEY CLUSTERED 
                (
                	[TruckClassificationID] ASC
                )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];

                ALTER TABLE [dbo].[LoadTimers]  WITH CHECK ADD  CONSTRAINT [FK_dbo.LoadTimers_dbo.BillTypes_BillTypeID] FOREIGN KEY([BillTypeID])
                REFERENCES [dbo].[BillTypes] ([BillTypeID]);
    
                ALTER TABLE [dbo].[LoadTimers] CHECK CONSTRAINT [FK_dbo.LoadTimers_dbo.BillTypes_BillTypeID];

                ALTER TABLE [dbo].[Pricings]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Pricings_dbo.BillTypes_BillTypeID] FOREIGN KEY([BillTypeID])
                REFERENCES [dbo].[BillTypes] ([BillTypeID])
                ON DELETE CASCADE;

                ALTER TABLE [dbo].[Pricings] CHECK CONSTRAINT [FK_dbo.Pricings_dbo.BillTypes_BillTypeID];

                ALTER TABLE [dbo].[Pricings]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Pricings_dbo.CustomerTypes_CustomerTypeID] FOREIGN KEY([CustomerTypeID])
                REFERENCES [dbo].[CustomerTypes] ([CustomerTypeID])
                ON DELETE CASCADE;

                ALTER TABLE [dbo].[Pricings] CHECK CONSTRAINT [FK_dbo.Pricings_dbo.CustomerTypes_CustomerTypeID];

                ALTER TABLE [dbo].[Jobs]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Jobs_dbo.CustomerTypes_CustomerTypeID] FOREIGN KEY([CustomerTypeID])
                REFERENCES [dbo].[CustomerTypes] ([CustomerTypeID]);

                ALTER TABLE [dbo].[Jobs] CHECK CONSTRAINT [FK_dbo.Jobs_dbo.CustomerTypes_CustomerTypeID];
            ");
        }
        
        public override void Down()
        {
        }
    }
}
