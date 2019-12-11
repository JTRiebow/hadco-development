namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedOpenTimerEntryConstraint : DbMigration
    {
        public override void Up()
        {
            // At the time of development, there is no data in production that will cause the below constraint to fail.
            // If the data changes before the release, however, we do no want to crash the whole app, since this constraint
            // isn't mission critical - just an extra saftey check against duplicate timers. If it fails, the constraint can 
            // be added manually after fixing the data. 
            try
            {
                var now = DateTimeOffset.Now;
                Sql($@"
create unique nonclustered index UC_UniqueActiveClockIn
  on EmployeeTimerEntries (EmployeeTimerID)
  where Clockout is null
        and Clockin > '{now}'");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error applying migration: " + e);   
            }

        }
        
        public override void Down()
        {
            try
            {
                Sql(@"drop index UC_UniqueActiveClockIn on EmployeeTimerEntries");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error applying migration down method: " + e);
            }
        }
    }
}
