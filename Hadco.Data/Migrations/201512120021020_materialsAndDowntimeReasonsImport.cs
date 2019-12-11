namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class materialsAndDowntimeReasonsImport : DbMigration
    {
        public override void Up()
        {
            Sql(@"insert into downtimereasons values ('ACCIDENT','DT1','15TKOH','GEN','DWN',  0);
insert into downtimereasons values ('BREAKDOWN - AIR LEAK','DT2','15TKOH','GEN','BRKDWN',  0);
insert into downtimereasons values ('BREAKDOWN - AIR LINE / BAG','DT3','15TKOH','GEN','BRKDWN',  0);
insert into downtimereasons values ('BREAKDOWN - BRAKES','DT4','15TKOH','GEN','BRKDWN',  0);
insert into downtimereasons values ('BREAKDOWN - DAMAGE','DT5','15TKOH','GEN','BRKDWN',  0);
insert into downtimereasons values ('BREAKDOWN - DRIVETRAIN','DT6','15TKOH','GEN','BRKDWN',  0);
insert into downtimereasons values ('BREAKDOWN - ENGINE','DT7','15TKOH','GEN','BRKDWN',  0);
insert into downtimereasons values ('BREAKDOWN - FLUIDS','DT8','15TKOH','GEN','BRKDWN',  0);
insert into downtimereasons values ('BREAKDOWN - FROZEN/AIRING UP','DT9','15TKOH','GEN','WINTER',  0);
insert into downtimereasons values ('BREAKDOWN - LIGHTS','DT10','15TKOH','GEN','BRKDWN',  0);
insert into downtimereasons values ('BREAKDOWN - OTHER','DT11','15TKOH','GEN','BRKDWN',  0);
insert into downtimereasons values ('BREAKDOWN - STARTING AIR UP','DT12','15TKOH','GEN','AIRUP',  0);
insert into downtimereasons values ('BREAKDOWN - WAIT ON MECH','DT13','15TKOH','GEN','BRKDWN',  0);
insert into downtimereasons values ('BREAKDOWN - WAIT ON PARTS','DT14','15TKOH','GEN','BRKDWN',  0);
insert into downtimereasons values ('CANCELED','DT15','15TKOH','GEN','DWN',  0);
insert into downtimereasons values ('CLEAN - BED','DT16','15TKOH','GEN','WASH',  0);
insert into downtimereasons values ('CLEAN - CAB','DT17','15TKOH','GEN','WASH',  0);
insert into downtimereasons values ('CLEAN - WASH','DT18','15TKOH','GEN','WASH',  0);
insert into downtimereasons values ('DISPATCH / ROLL CALL','DT19','15TKOH','GEN','DIS',  0);
insert into downtimereasons values ('DROP/HOOK TRAILERS','DT20','15TKOH','GEN','MISC',  0);
insert into downtimereasons values ('EQUIPMENT - BREAKDOWN','DT21',NULL,'GEN','DWN',  1);
insert into downtimereasons values ('EQUIPMENT - FUEL','DT22',NULL,'GEN','DWN',  1);
insert into downtimereasons values ('EQUIPMENT - SERVICE','DT23',NULL,'GEN','DWN',  1);
insert into downtimereasons values ('EQUIPMENT - TRANSPORT','DT24',NULL,'GEN','DWN',  1);
insert into downtimereasons values ('INSPECTION - D.O.T.','DT25','15TKOH','GEN','INSP',  0);
insert into downtimereasons values ('INSPECTION - SAFETY DEPT','DT26','15TKOH','GEN','INSP',  0);
insert into downtimereasons values ('INSPECTION - STATE SAFETY','DT27','15TKOH','GEN','INSP',  0);
insert into downtimereasons values ('JOB - COMPLETE','DT28',NULL,'GEN','DWN',  1);
insert into downtimereasons values ('JOB - WAIT TO DUMP - OPERATOR','DT29',NULL,'GEN','DWN',  1);
insert into downtimereasons values ('JOB - WAIT TO DUMP - OTHER','DT30',NULL,'GEN','DWN',  1);
insert into downtimereasons values ('JOB - WAIT TO DUMP - TRUCK LINE','DT31',NULL,'GEN','DWN',  1);
insert into downtimereasons values ('JOB - WAIT TO LOAD - OPERATOR','DT32',NULL,'GEN','DWN',  1);
insert into downtimereasons values ('JOB - WAIT TO LOAD - OTHER','DT33',NULL,'GEN','DWN',  1);
insert into downtimereasons values ('JOB - WAIT TO LOAD - TRUCK LINE','DT34',NULL,'GEN','DWN',  1);
insert into downtimereasons values ('LABORING','DT35','15TKOH','GEN','LABOR',  0);
insert into downtimereasons values ('MAINT - BRAKES','DT36','15TKOH','GEN','BRAKE',  0);
insert into downtimereasons values ('MAINT - FLUIDS','DT37','15TKOH','GEN','BRKDWN',  0);
insert into downtimereasons values ('MAINT - GREASE','DT38','15TKOH','GEN','GREASE',  0);
insert into downtimereasons values ('MAINT - LIGHTS','DT39','15TKOH','GEN','BRKDWN',  0);
insert into downtimereasons values ('MAINT - MINOR FIXES','DT40','15TKOH','GEN','BRKDWN',  0);
insert into downtimereasons values ('MEETING','DT41','15TKOH','GEN','MTG',  0);
insert into downtimereasons values ('MOVE TRAILER','DT42',NULL,'PHASE','CAT',  1);
insert into downtimereasons values ('PIT - DOWN OTHER','DT43','15TKOH','GEN','PIT',  0);
insert into downtimereasons values ('PIT - RELOAD','DT44','15TKOH','GEN','PIT',  0);
insert into downtimereasons values ('PIT - WAIT TO DUMP','DT45','15TKOH','GEN','PIT',  0);
insert into downtimereasons values ('PIT - WAIT TO LOAD','DT46','15TKOH','GEN','PIT',  0);
insert into downtimereasons values ('TRAINING/ROAD TEST / CDL CLASS','DT47','15TKOH','GEN','TRAIN',  0);
insert into downtimereasons values ('SHOP/YARD','DT48','15HADCO','SHOP','YARD',  0);
insert into downtimereasons values ('TESTING/COMPACTION','DT49',NULL,'GEN','DWN',  1);
insert into downtimereasons values ('TIRES - AIR UP','DT50','15TKOH','GEN','TIRES',  0);
insert into downtimereasons values ('TIRES - ALIGNMENT','DT51','15TKOH','GEN','TIRES',  0);
insert into downtimereasons values ('TIRES - BLOWN','DT52','15TKOH','GEN','TIRES',  0);
insert into downtimereasons values ('TIRES - CHECK','DT53','15TKOH','GEN','TIRES',  0);
insert into downtimereasons values ('TIRES - REPAIR','DT54','15TKOH','GEN','TIRES',  0);
insert into downtimereasons values ('GATE CHECK','DT55','15TKOH','GEN','GATE',  0);
insert into downtimereasons values ('TRANSPORT DRIVER','DT56',NULL,'PHASE','CAT',  1);
insert into downtimereasons values ('WAIT ON OPERATOR','DT57',NULL,'GEN','DWN',  1);
insert into downtimereasons values ('LOAD SELF','DT58',NULL,'PHASE','CAT',  1);
insert into downtimereasons values ('LOAD TRUCKS','DT59',NULL,'PHASE','CAT',  1);
insert into downtimereasons values ('STUCK (IN MUD)','DT60','15TKOH','GEN','DWN',  0);
insert into downtimereasons values ('LOAD STUCK','DT61','15TKOH','GEN','DWN',  0);
insert into downtimereasons values ('DRIVER SPECIAL','DT62','15TKOH','GEN','DWN',  0);
insert into downtimereasons values ('SWEEPER','DT63',NULL,'TRCK','SWEP',  1);
insert into downtimereasons values ('LIGHT DUTY','DT64','15TKOH','GEN','LD',  0);
insert into downtimereasons values ('CLEAN - BED SLINGER','DT65','15TKOH','GEN','DWN',  0);
insert into downtimereasons values ('MAINLINE - PRELOAD','DT66',NULL,'PHASE','CAT',  1);
insert into downtimereasons values ('FUELING SEMI IN YARD','DT67','15TKOH','GEN','YFUEL',  0);
insert into downtimereasons values ('MANAGER','DT68','15TKOH','GEN','MGR',  0);");

            Sql(@"insert into materials values ('1 1/2 GRAVEL','GRAV');
insert into materials values('1 1/2 WASH COBBLE', 'COBB');
            insert into materials values ('1/2 MINUS', 'GRAV');
            insert into materials values ('2IN GRAVEL', 'GRAV');
            insert into materials values ('3 MINUS', 'BRUN');
            insert into materials values ('3-6"" WASATCH GREY', 'BRUN');
            insert into materials values ('7/16 GRAVEL', 'GRAV');
            insert into materials values ('A 1 B', 'BRUN');
            insert into materials values ('ASPHALT', 'GARB');
            insert into materials values ('ASPHALT/CON', 'GARB');
            insert into materials values ('BANK RUN', 'BRUN');
            insert into materials values ('BANK RUN 1', 'BRUN');
            insert into materials values ('BEDDING SAND', 'SAND');
            insert into materials values ('BOULDERS', 'LROC');
            insert into materials values ('BROWN BANK', 'BRUN');
            insert into materials values ('CLAY', 'CLAY');
            insert into materials values ('COBBLE', 'COBB');
            insert into materials values ('CONCRETE', 'GARB');
            insert into materials values ('CONCRETE ROADBASE', NULL);
            insert into materials values ('CONCRETE SAND', 'SAND');
            insert into materials values ('CRUSHED CONCRETE', 'SAND');
            insert into materials values ('CRUSHER FINES', 'BRUN');
            insert into materials values ('CYCLONE SAND', 'SAND');
            insert into materials values ('DIRT', 'HAUL');
            insert into materials values ('E FILL', 'BRUN');
            insert into materials values ('FINES', 'BRUN');
            insert into materials values ('FORMS', 'WALL');
            insert into materials values ('GARBAGE', 'GARB');
            insert into materials values ('GRAVEL', 'GRAV');
            insert into materials values ('GRAVEL 1', 'GRAV');
            insert into materials values ('GRAVEL 2', 'GRAV');
            insert into materials values ('GREY BANKRUN', 'BRUN');
            insert into materials values ('HARD PACK', 'BRUN');
            insert into materials values ('HED', 'HAUL');
            insert into materials values ('HOT ASPHALT', 'ASPH');
            insert into materials values ('HS', 'HAUL');
            insert into materials values ('HTP', 'HAUL');
            insert into materials values ('LOGS', 'GARB');
            insert into materials values ('MASON SAND', 'SAND');
            insert into materials values ('MOB', NULL);
            insert into materials values ('MUD', 'MUD');
            insert into materials values ('NONE', NULL);
            insert into materials values ('OFF DIRT', 'HAUL');
            insert into materials values ('ON DIRT', 'HAUL');
            insert into materials values ('PEA GRAVEL', 'PGRV');
            insert into materials values ('PEA GRAVEL 1', 'PGRV');
            insert into materials values ('REC ASPHALT', NULL);
            insert into materials values ('RED 3', 'BRUN');
            insert into materials values ('REJ 3/4 GRAVEL', 'GRAV');
            insert into materials values ('REJ SAND', 'BRUN');
            insert into materials values ('RIP RAP', 'COBB');
            insert into materials values ('ROADBASE', 'UTBC');
            insert into materials values ('ROADBASE 2', 'UTBC');
            insert into materials values ('ROCKS', 'HAUL');
            insert into materials values ('SALT', NULL);
            insert into materials values ('SAND', 'SAND');
            insert into materials values ('SAND 2', 'SAND');
            insert into materials values ('SAND/GRAVEL', 'HAUL');
            insert into materials values ('SL GRAVEL', 'GRAV');
            insert into materials values ('SL PEA GRAVEL', 'PGRV');
            insert into materials values ('SLING', NULL);
            insert into materials values ('S FILL', 'BRUN');
            insert into materials values ('SPECIAL FILL', 'BANK');
            insert into materials values ('SWEEP', 'SWEP');
            insert into materials values ('TOP SOIL', 'SOIL');
            insert into materials values ('TREES', 'GARB');
            insert into materials values ('TYPE 3 SAND', 'SAND');
            insert into materials values ('WASHED SAND', 'SAND');
            insert into materials values ('WATER', 'DUST');");

        }
        
        public override void Down()
        {
        }
    }
}
