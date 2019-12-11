using Hadco.Data.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Hadco.Common;

namespace Hadco.Data
{
    public class HadcoContext : DbContext
    {
        public HadcoContext()
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<HadcoContext, Data.Migrations.Configuration>());
        }

        public override int SaveChanges()
        {
            var trackedEntities = ChangeTracker.Entries<TrackedEntity>()?.ToList();
            if (trackedEntities != null && trackedEntities.Any())
            {
                var now = DateTimeOffset.Now;
                var currentUser = HttpContext.Current?.User as ClaimsPrincipal;
                var currentEmployeeID = currentUser?.GetEmployeeID();
                foreach (var item in trackedEntities.Where(t => t.State == EntityState.Added))
                {
                    item.Entity.CreatedOn = now;
                    item.Entity.UpdatedOn = now;
                    item.Entity.CreatedEmployeeID = currentEmployeeID;
                    item.Entity.ModifiedEmployeeID = currentEmployeeID;
                }
                foreach (var item in trackedEntities.Where(t => t.State == EntityState.Modified))
                {
                    item.Entity.UpdatedOn = now;
                    item.Entity.ModifiedEmployeeID = currentEmployeeID;
                }
            }
            return base.SaveChanges();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EmployeeTimerEntry>().Property(x => x.ClockInLatitude).HasPrecision(9, 6);
            modelBuilder.Entity<EmployeeTimerEntry>().Property(x => x.ClockInLongitude).HasPrecision(9, 6);
            modelBuilder.Entity<EmployeeTimerEntry>().Property(x => x.ClockOutLatitude).HasPrecision(9, 6);
            modelBuilder.Entity<EmployeeTimerEntry>().Property(x => x.ClockOutLongitude).HasPrecision(9, 6);

            modelBuilder.Entity<LoadTimerEntry>().Property(x => x.StartTimeLatitude).HasPrecision(9, 6);
            modelBuilder.Entity<LoadTimerEntry>().Property(x => x.StartTimeLongitude).HasPrecision(9, 6);
            modelBuilder.Entity<LoadTimerEntry>().Property(x => x.EndTimeLatitude).HasPrecision(9, 6);
            modelBuilder.Entity<LoadTimerEntry>().Property(x => x.EndTimeLongitude).HasPrecision(9, 6);

            modelBuilder.Entity<Location>().Property(x => x.Latitude).HasPrecision(9, 6);
            modelBuilder.Entity<Location>().Property(x => x.Longitude).HasPrecision(9, 6);;

            modelBuilder.Entity<EmployeeTimer>()
                .HasMany(x => x.Occurrences)
                .WithMany()
                .Map(x => x.MapLeftKey("EmployeeTimerID").MapRightKey("OccurrenceID").ToTable("EmployeTimerOccurrences"));

            modelBuilder.Entity<Employee>()
                .HasMany(x => x.Roles)
                .WithMany()
                .Map(x => x.MapLeftKey("EmployeeID").MapRightKey("RoleId").ToTable("EmployeeRoles"));

            modelBuilder.Entity<Employee>()
                .HasMany(x => x.Departments)
                .WithMany(x => x.Employees)
                .Map(x => x.MapLeftKey("EmployeeID").MapRightKey("DepartmentID").ToTable("EmployeeDepartments"));

            modelBuilder.Entity<Employee>()
                .HasMany(x => x.Supervisors)
                .WithMany(x => x.Employees)
                .Map(x => x.MapLeftKey("EmployeeID").MapRightKey("SupervisorID").ToTable("EmployeeSupervisors"));

            modelBuilder.Entity<Employee>()
                .HasOptional(x => x.ClockedInStatus);
                
            modelBuilder.Entity<ClockedInStatus>()
                .HasKey(x => x.EmployeeID)
                .HasRequired(x=>x.Employee);

            modelBuilder.Entity<Resource>()
                .Map(x => x.ToTable("Resources"));

            modelBuilder.Entity<RoleResource>()
                .Map(x => x.ToTable("RoleResources"));

            modelBuilder.Entity<RoleAuthActivity>()
               .HasMany(x => x.Departments)
               .WithMany()
               .Map(x => x.MapLeftKey("RoleAuthActivityID").MapRightKey("DepartmentID").ToTable("RoleAuthActivityDepartments"));

        }

        public DbSet<Role> Roles { get; set; }
        public DbSet<RoleResource> RoleResources { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<Equipment> Equipment { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<Phase> Phases { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }

        public DbSet<Timesheet> Timesheets { get; set; }        
        public DbSet<JobTimer> JobTimers { get; set; }
        public DbSet<EmployeeTimer> EmployeeTimers { get; set; }
        public DbSet<EmployeeTimerEntryHistory> EmployeeTimerEntryHistories { get; set; }
        public DbSet<Occurrence> Occurrences { get; set; }
        public DbSet<EmployeeJobTimer> EmployeeJobTimers { get; set; }
        public DbSet<EmployeeJobEquipmentTimer> EmployeeJobEquipmentTimers { get; set; }
        public DbSet<EmployeeTimerEntry> EmployeeTimerEntries { get; set; }
        public DbSet<EmployeeTimecard> EmployeeTimercards { get; set; }
        public DbSet<EmployeeType> EmployeeTypes { get; set; }

        public DbSet<EquipmentServiceType> EquipmentServiceTypes { get; set; }
        public DbSet<EquipmentTimer> EquipmentTimers { get; set; }
        public DbSet<EquipmentTimerEntry> EquipmentTimerEntries { get; set; }

        public DbSet<LoadTimer> LoadTimers { get; set; }
        public DbSet<LoadTimerEntry> LoadTimerEntries { get; set; }
        public DbSet<BillType> BillTypes { get; set; }
        public DbSet<DowntimeReason> DowntimeReasons { get; set; }
        public DbSet<Material> Materials { get; set; }
        public DbSet<DowntimeTimer> DowntimeTimers { get; set; }
        public DbSet<CustomerType> CustomerTypes { get; set; }
        public DbSet<TruckClassification> TruckClassifications { get; set; }
        public DbSet<Price> Prices { get; set; }
        public DbSet<Pricing> Pricings { get; set; }
        public DbSet<Pit> Pits { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<OverheadCode> OverheadCodes { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<Note> Notes { get; set; }
        public DbSet<ClockedInStatus> ClockedInStatuses { get; set; }
        public DbSet<NoteType> NoteTypes { get; set; }
        public DbSet<DailyApproval> DailyApprovals { get; set; }
        public DbSet<RoleAuthActivity> RoleAuthActivities { get; set; }
        public DbSet<AuthActivity> AuthActivities { get; set; }
        public DbSet<AuthSection> AuthSections { get; set; }

    }
}
