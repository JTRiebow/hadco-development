using System.Data.Entity.Infrastructure;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Serialization;
using Dapper;
using Hadco.Common.Enums;

namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Data.Entity.Validation;
    using System.Linq;
    using System.Text;
    using Hadco.Data.Entities;
    using Hadco.Common;
    using System.Collections.Generic;

    internal sealed class Configuration : DbMigrationsConfiguration<HadcoContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            CommandTimeout = 10000;
        }

        protected override void Seed(HadcoContext context)
        {
            DoSeed(context);
        }

        private void DoSeed(HadcoContext context)
        {
            //if (System.Diagnostics.Debugger.IsAttached == false)
            //    System.Diagnostics.Debugger.Launch();
            #region Roles

            Role sysAdmin = new Role() { ID = 1, Name = ProjectConstants.AdminRole };
            Role webUser = new Role() { ID = 2, Name = ProjectConstants.WebUserRole };
            Role supervisor = new Role() { ID = 3, Name = ProjectConstants.SupervisorRole };
            Role accounting = new Role() { ID = 4, Name = ProjectConstants.AccountingRole };
            Role truckingRole = new Role() { ID = 5, Name = ProjectConstants.TruckingRole };
            Role truckingReports = new Role() { ID = 6, Name = ProjectConstants.TruckingReportsRole };
            Role foreman = new Role() { ID = 7, Name = ProjectConstants.ForemanRole };
            Role superintendent = new Role() { ID = 8, Name = ProjectConstants.SuperintendentRole };
            Role billing = new Role() { ID = 9, Name = ProjectConstants.BillingRole };
            Role HR = new Role() { ID = 10, Name = ProjectConstants.HRRole };
            Role billingAdmin = new Role() { ID = 11, Name = ProjectConstants.BillingAdminRole };
            Role truckingAdmin = new Role() { ID = 12, Name = ProjectConstants.TruckingAdminRole };
            Role mobileUser = new Role() { ID = 13, Name = ProjectConstants.MobileUserRole };
            Role manager = new Role() { ID = 14, Name = ProjectConstants.ManagerRole };
            Role maintenanceAdmin = new Role() { ID = 15, Name = ProjectConstants.MaintenanceAdminRole };
            Role scheduling = new Role() { ID = 16, Name = ProjectConstants.Scheduling };

            //var roles = new Role[] { sysAdmin, webUser, supervisor, accounting
            //    , truckingRole, truckingReports, foreman, superintendent
            //    , billing, HR, billingAdmin, truckingAdmin
            //    , mobileUser, manager, maintenanceAdmin, scheduling };

            //foreach (Role role in roles)
            //{
            //    context.Roles.AddOrUpdate(role);
            //}

            //context.SaveChanges();

            //using raw sql here to prevent errors caused by trying to rely on the ID of inserted role when the ID is database generated.
            context.Database.ExecuteSqlCommand($@"
                begin transaction;
                set identity_insert dbo.Roles on;
                with source as
                (
                  select {sysAdmin.ID} as RoleID, N'{sysAdmin.Name}' as Name
                  union
                  select {webUser.ID} as RoleID, N'{webUser.Name}' as Name
                  union
                  select {supervisor.ID} as RoleID, N'{supervisor.Name}' as Name
                  union
                  select {accounting.ID} as RoleID, N'{accounting.Name}' as Name
                  union
                  select {truckingRole.ID} as RoleID, N'{truckingRole.Name}' as Name
                  union
                  select {truckingReports.ID} as RoleID, N'{truckingReports.Name}' as Name
                  union
                  select {foreman.ID} as RoleID, N'{foreman.Name}' as Name
                  union
                  select {superintendent.ID} as RoleID, N'{superintendent.Name}' as Name
                  union
                  select {billing.ID} as RoleID, N'{billing.Name}' as Name
                  union
                  select {HR.ID} as RoleID, N'{HR.Name}' as Name
                  union
                  select {billingAdmin.ID} as RoleID, N'{billingAdmin.Name}' as Name
                  union
                  select {truckingAdmin.ID} as RoleID, N'{truckingAdmin.Name}' as Name
                  union
                  select {mobileUser.ID} as RoleID, N'{mobileUser.Name}' as Name
                  union
                  select {manager.ID} as RoleID, N'{manager.Name}' as Name
                  union
                  select {maintenanceAdmin.ID} as RoleID, N'{maintenanceAdmin.Name}' as Name
                  union
                  select {scheduling.ID} as RoleID, N'{scheduling.Name}' as Name
                )
                merge into Roles as target
                using source
                on target.RoleID = source.RoleID
                when not matched by source then delete
                when matched
                  and target.Name != source.Name then update set target.Name = source.Name
                when not matched by target then insert (RoleID, Name) values (source.RoleID, source.Name);

                set identity_insert dbo.Roles off;
                commit transaction");

            #endregion Roles

            #region Resources

            context.Resources.AddOrUpdate(x => x.ID,
                new Resource() { ID = 1, Name = "Roles" },
                new Resource() { ID = 2, Name = "Resources" },
                new Resource() { ID = 3, Name = "Users" },
                new Resource() { ID = 4, Name = "Categories" },
                new Resource() { ID = 5, Name = "Customers" },
                new Resource() { ID = 6, Name = "Departments" },
                new Resource() { ID = 7, Name = "Employees" },
                new Resource() { ID = 8, Name = "Equipment" },
                new Resource() { ID = 9, Name = "Jobs" },
                new Resource() { ID = 10, Name = "Phases" },
                new Resource() { ID = 11, Name = "Units" },
                new Resource() { ID = 12, Name = "Occurrences" },
                new Resource() { ID = 13, Name = "Timesheets" },
                new Resource() { ID = 14, Name = "JobTimers" },
                new Resource() { ID = 15, Name = "EmployeeTimers" },
                new Resource() { ID = 16, Name = "EmployeeJobTimers" },
                new Resource() { ID = 17, Name = "EmployeeTimerEntries" },
                new Resource() { ID = 18, Name = "EquipmentTimers" },
                new Resource() { ID = 19, Name = "EquipmentServiceTypes" },
                new Resource() { ID = 20, Name = "EmployeeTimecards" },
                new Resource() { ID = 21, Name = "EmployeeJobEquipmentTimers" },
                new Resource() { ID = 22, Name = "LoadTimers" },
                new Resource() { ID = 23, Name = "Materials" },
                new Resource() { ID = 24, Name = "DowntimeReasons" },
                new Resource() { ID = 25, Name = "DowntimeTimers" },
                new Resource() { ID = 26, Name = "EmployeeTypes" },
                new Resource() { ID = 27, Name = "EntityStatuses" },
                new Resource() { ID = 28, Name = "BillTypes" },
                new Resource() { ID = 29, Name = "CustomerTypes" },
                new Resource() { ID = 30, Name = "TruckClassifications" },
                new Resource() { ID = 31, Name = "Pricings" },
                new Resource() { ID = 32, Name = "Prices" },
                new Resource() { ID = 34, Name = "Pits" },
                new Resource() { ID = 35, Name = "LoadTimerEntries" },
                new Resource() { ID = 36, Name = "Locations" },
                new Resource() { ID = 37, Name = "EquipmentTimerEntries" },
                new Resource() { ID = 38, Name = "Settings" },
                new Resource() { ID = 39, Name = "RoleAuthActivities" },
                new Resource() { ID = 40, Name = "AuthActivities" }
            );

            #endregion Resources

            #region RoleResources

            context.RoleResources.AddOrUpdate(
                new RoleResource() { RoleID = sysAdmin.ID, ResourceID = 1, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = sysAdmin.ID, ResourceID = 2, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = sysAdmin.ID, ResourceID = 3, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = sysAdmin.ID, ResourceID = 4, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = sysAdmin.ID, ResourceID = 5, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = sysAdmin.ID, ResourceID = 6, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = sysAdmin.ID, ResourceID = 7, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = sysAdmin.ID, ResourceID = 8, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = sysAdmin.ID, ResourceID = 9, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = sysAdmin.ID, ResourceID = 10, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = sysAdmin.ID, ResourceID = 11, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = sysAdmin.ID, ResourceID = 12, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = sysAdmin.ID, ResourceID = 13, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = sysAdmin.ID, ResourceID = 14, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = sysAdmin.ID, ResourceID = 15, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = sysAdmin.ID, ResourceID = 16, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = sysAdmin.ID, ResourceID = 17, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = sysAdmin.ID, ResourceID = 18, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = sysAdmin.ID, ResourceID = 19, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = sysAdmin.ID, ResourceID = 20, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = sysAdmin.ID, ResourceID = 21, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = sysAdmin.ID, ResourceID = 22, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = sysAdmin.ID, ResourceID = 23, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = sysAdmin.ID, ResourceID = 24, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = sysAdmin.ID, ResourceID = 25, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = sysAdmin.ID, ResourceID = 26, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = sysAdmin.ID, ResourceID = 27, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = sysAdmin.ID, ResourceID = 28, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = sysAdmin.ID, ResourceID = 29, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = sysAdmin.ID, ResourceID = 30, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = sysAdmin.ID, ResourceID = 31, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = sysAdmin.ID, ResourceID = 32, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = sysAdmin.ID, ResourceID = 34, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = sysAdmin.ID, ResourceID = 35, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = sysAdmin.ID, ResourceID = 36, Read = true, Write = true, Delete = false },
                new RoleResource() { RoleID = sysAdmin.ID, ResourceID = 37, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = sysAdmin.ID, ResourceID = 38, Read = true, Write = true, Delete = false },
                new RoleResource() { RoleID = sysAdmin.ID, ResourceID = 39, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = sysAdmin.ID, ResourceID = 40, Read = true, Write = false, Delete = false }
            );

            context.RoleResources.AddOrUpdate(
                new RoleResource() { RoleID = webUser.ID, ResourceID = 1, Read = false, Write = false, Delete = false },
                new RoleResource() { RoleID = webUser.ID, ResourceID = 2, Read = false, Write = false, Delete = false },
                new RoleResource() { RoleID = webUser.ID, ResourceID = 3, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = webUser.ID, ResourceID = 4, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = webUser.ID, ResourceID = 5, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = webUser.ID, ResourceID = 6, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = webUser.ID, ResourceID = 7, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = webUser.ID, ResourceID = 8, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = webUser.ID, ResourceID = 9, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = webUser.ID, ResourceID = 10, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = webUser.ID, ResourceID = 11, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = webUser.ID, ResourceID = 12, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = webUser.ID, ResourceID = 13, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = webUser.ID, ResourceID = 14, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = webUser.ID, ResourceID = 15, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = webUser.ID, ResourceID = 16, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = webUser.ID, ResourceID = 17, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = webUser.ID, ResourceID = 18, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = webUser.ID, ResourceID = 19, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = webUser.ID, ResourceID = 20, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = webUser.ID, ResourceID = 21, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = webUser.ID, ResourceID = 22, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = webUser.ID, ResourceID = 23, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = webUser.ID, ResourceID = 24, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = webUser.ID, ResourceID = 25, Read = true, Write = true, Delete = false },
                new RoleResource() { RoleID = webUser.ID, ResourceID = 26, Read = false, Write = false, Delete = false },
                new RoleResource() { RoleID = webUser.ID, ResourceID = 27, Read = false, Write = false, Delete = false },
                new RoleResource() { RoleID = webUser.ID, ResourceID = 28, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = webUser.ID, ResourceID = 29, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = webUser.ID, ResourceID = 30, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = webUser.ID, ResourceID = 31, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = webUser.ID, ResourceID = 32, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = webUser.ID, ResourceID = 34, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = webUser.ID, ResourceID = 35, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = webUser.ID, ResourceID = 36, Read = false, Write = true, Delete = false },
                new RoleResource() { RoleID = webUser.ID, ResourceID = 37, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = webUser.ID, ResourceID = 38, Read = false, Write = false, Delete = false },
                new RoleResource() { RoleID = webUser.ID, ResourceID = 39, Read = false, Write = false, Delete = false },
                new RoleResource() { RoleID = webUser.ID, ResourceID = 40, Read = false, Write = false, Delete = false }
            );

            context.RoleResources.AddOrUpdate(
                new RoleResource() { RoleID = supervisor.ID, ResourceID = 1, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = supervisor.ID, ResourceID = 2, Read = false, Write = false, Delete = false },
                new RoleResource() { RoleID = supervisor.ID, ResourceID = 3, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = supervisor.ID, ResourceID = 4, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = supervisor.ID, ResourceID = 5, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = supervisor.ID, ResourceID = 6, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = supervisor.ID, ResourceID = 7, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = supervisor.ID, ResourceID = 8, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = supervisor.ID, ResourceID = 9, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = supervisor.ID, ResourceID = 10, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = supervisor.ID, ResourceID = 11, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = supervisor.ID, ResourceID = 12, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = supervisor.ID, ResourceID = 13, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = supervisor.ID, ResourceID = 14, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = supervisor.ID, ResourceID = 15, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = supervisor.ID, ResourceID = 16, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = supervisor.ID, ResourceID = 17, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = supervisor.ID, ResourceID = 18, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = supervisor.ID, ResourceID = 19, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = supervisor.ID, ResourceID = 20, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = supervisor.ID, ResourceID = 21, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = supervisor.ID, ResourceID = 22, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = supervisor.ID, ResourceID = 23, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = supervisor.ID, ResourceID = 24, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = supervisor.ID, ResourceID = 25, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = supervisor.ID, ResourceID = 26, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = supervisor.ID, ResourceID = 27, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = supervisor.ID, ResourceID = 28, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = supervisor.ID, ResourceID = 29, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = supervisor.ID, ResourceID = 30, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = supervisor.ID, ResourceID = 31, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = supervisor.ID, ResourceID = 32, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = supervisor.ID, ResourceID = 34, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = supervisor.ID, ResourceID = 35, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = supervisor.ID, ResourceID = 36, Read = true, Write = true, Delete = false },
                new RoleResource() { RoleID = supervisor.ID, ResourceID = 37, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = supervisor.ID, ResourceID = 38, Read = false, Write = false, Delete = false },
                new RoleResource() { RoleID = supervisor.ID, ResourceID = 39, Read = false, Write = false, Delete = false },
                new RoleResource() { RoleID = supervisor.ID, ResourceID = 40, Read = false, Write = false, Delete = false }
            );

            context.RoleResources.AddOrUpdate(
                new RoleResource() { RoleID = accounting.ID, ResourceID = 1, Read = false, Write = false, Delete = false },
                new RoleResource() { RoleID = accounting.ID, ResourceID = 2, Read = false, Write = false, Delete = false },
                new RoleResource() { RoleID = accounting.ID, ResourceID = 3, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = accounting.ID, ResourceID = 4, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = accounting.ID, ResourceID = 5, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = accounting.ID, ResourceID = 6, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = accounting.ID, ResourceID = 7, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = accounting.ID, ResourceID = 8, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = accounting.ID, ResourceID = 9, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = accounting.ID, ResourceID = 10, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = accounting.ID, ResourceID = 11, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = accounting.ID, ResourceID = 12, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = accounting.ID, ResourceID = 13, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = accounting.ID, ResourceID = 14, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = accounting.ID, ResourceID = 15, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = accounting.ID, ResourceID = 16, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = accounting.ID, ResourceID = 17, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = accounting.ID, ResourceID = 18, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = accounting.ID, ResourceID = 19, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = accounting.ID, ResourceID = 20, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = accounting.ID, ResourceID = 21, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = accounting.ID, ResourceID = 22, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = accounting.ID, ResourceID = 23, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = accounting.ID, ResourceID = 24, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = accounting.ID, ResourceID = 25, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = accounting.ID, ResourceID = 26, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = accounting.ID, ResourceID = 27, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = accounting.ID, ResourceID = 28, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = accounting.ID, ResourceID = 29, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = accounting.ID, ResourceID = 30, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = accounting.ID, ResourceID = 31, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = accounting.ID, ResourceID = 32, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = accounting.ID, ResourceID = 34, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = accounting.ID, ResourceID = 35, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = accounting.ID, ResourceID = 36, Read = true, Write = true, Delete = false },
                new RoleResource() { RoleID = accounting.ID, ResourceID = 37, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = accounting.ID, ResourceID = 38, Read = false, Write = false, Delete = false },
                new RoleResource() { RoleID = accounting.ID, ResourceID = 39, Read = false, Write = false, Delete = false },
                new RoleResource() { RoleID = accounting.ID, ResourceID = 40, Read = false, Write = false, Delete = false }
            );

            context.RoleResources.AddOrUpdate(
                 new RoleResource() { RoleID = truckingRole.ID, ResourceID = 1, Read = false, Write = false, Delete = false },
                 new RoleResource() { RoleID = truckingRole.ID, ResourceID = 2, Read = false, Write = false, Delete = false },
                 new RoleResource() { RoleID = truckingRole.ID, ResourceID = 3, Read = true, Write = false, Delete = false },
                 new RoleResource() { RoleID = truckingRole.ID, ResourceID = 4, Read = true, Write = false, Delete = false },
                 new RoleResource() { RoleID = truckingRole.ID, ResourceID = 5, Read = true, Write = false, Delete = false },
                 new RoleResource() { RoleID = truckingRole.ID, ResourceID = 6, Read = true, Write = false, Delete = false },
                 new RoleResource() { RoleID = truckingRole.ID, ResourceID = 7, Read = true, Write = false, Delete = false },
                 new RoleResource() { RoleID = truckingRole.ID, ResourceID = 8, Read = true, Write = false, Delete = false },
                 new RoleResource() { RoleID = truckingRole.ID, ResourceID = 9, Read = true, Write = false, Delete = false },
                 new RoleResource() { RoleID = truckingRole.ID, ResourceID = 10, Read = true, Write = false, Delete = false },
                 new RoleResource() { RoleID = truckingRole.ID, ResourceID = 11, Read = true, Write = false, Delete = false },
                 new RoleResource() { RoleID = truckingRole.ID, ResourceID = 12, Read = true, Write = false, Delete = false },
                 new RoleResource() { RoleID = truckingRole.ID, ResourceID = 13, Read = true, Write = true, Delete = true },
                 new RoleResource() { RoleID = truckingRole.ID, ResourceID = 14, Read = true, Write = true, Delete = true },
                 new RoleResource() { RoleID = truckingRole.ID, ResourceID = 15, Read = true, Write = true, Delete = true },
                 new RoleResource() { RoleID = truckingRole.ID, ResourceID = 16, Read = true, Write = true, Delete = true },
                 new RoleResource() { RoleID = truckingRole.ID, ResourceID = 17, Read = true, Write = true, Delete = true },
                 new RoleResource() { RoleID = truckingRole.ID, ResourceID = 18, Read = true, Write = true, Delete = true },
                 new RoleResource() { RoleID = truckingRole.ID, ResourceID = 19, Read = true, Write = false, Delete = false },
                 new RoleResource() { RoleID = truckingRole.ID, ResourceID = 20, Read = true, Write = true, Delete = true },
                 new RoleResource() { RoleID = truckingRole.ID, ResourceID = 21, Read = true, Write = true, Delete = true },
                 new RoleResource() { RoleID = truckingRole.ID, ResourceID = 22, Read = true, Write = true, Delete = true },
                 new RoleResource() { RoleID = truckingRole.ID, ResourceID = 23, Read = true, Write = false, Delete = false },
                 new RoleResource() { RoleID = truckingRole.ID, ResourceID = 24, Read = true, Write = false, Delete = false },
                 new RoleResource() { RoleID = truckingRole.ID, ResourceID = 25, Read = true, Write = true, Delete = true },
                 new RoleResource() { RoleID = truckingRole.ID, ResourceID = 26, Read = true, Write = false, Delete = false },
                 new RoleResource() { RoleID = truckingRole.ID, ResourceID = 27, Read = true, Write = false, Delete = false },
                 new RoleResource() { RoleID = truckingRole.ID, ResourceID = 28, Read = true, Write = false, Delete = false },
                 new RoleResource() { RoleID = truckingRole.ID, ResourceID = 29, Read = true, Write = false, Delete = false },
                 new RoleResource() { RoleID = truckingRole.ID, ResourceID = 30, Read = true, Write = false, Delete = false },
                 new RoleResource() { RoleID = truckingRole.ID, ResourceID = 31, Read = true, Write = false, Delete = false },
                 new RoleResource() { RoleID = truckingRole.ID, ResourceID = 32, Read = true, Write = false, Delete = false },
                 new RoleResource() { RoleID = truckingRole.ID, ResourceID = 34, Read = true, Write = false, Delete = false },
                 new RoleResource() { RoleID = truckingRole.ID, ResourceID = 35, Read = true, Write = true, Delete = true },
                 new RoleResource() { RoleID = truckingRole.ID, ResourceID = 36, Read = true, Write = true, Delete = false },
                 new RoleResource() { RoleID = truckingRole.ID, ResourceID = 37, Read = true, Write = true, Delete = true },
                 new RoleResource() { RoleID = truckingRole.ID, ResourceID = 38, Read = false, Write = false, Delete = false },
                 new RoleResource() { RoleID = truckingRole.ID, ResourceID = 39, Read = false, Write = false, Delete = false },
                 new RoleResource() { RoleID = truckingRole.ID, ResourceID = 40, Read = false, Write = false, Delete = false }
            );

            context.RoleResources.AddOrUpdate(
                new RoleResource() { RoleID = foreman.ID, ResourceID = 1, Read = false, Write = false, Delete = false },
                new RoleResource() { RoleID = foreman.ID, ResourceID = 2, Read = false, Write = false, Delete = false },
                new RoleResource() { RoleID = foreman.ID, ResourceID = 3, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = foreman.ID, ResourceID = 4, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = foreman.ID, ResourceID = 5, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = foreman.ID, ResourceID = 6, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = foreman.ID, ResourceID = 7, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = foreman.ID, ResourceID = 8, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = foreman.ID, ResourceID = 9, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = foreman.ID, ResourceID = 10, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = foreman.ID, ResourceID = 11, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = foreman.ID, ResourceID = 12, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = foreman.ID, ResourceID = 13, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = foreman.ID, ResourceID = 14, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = foreman.ID, ResourceID = 15, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = foreman.ID, ResourceID = 16, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = foreman.ID, ResourceID = 17, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = foreman.ID, ResourceID = 18, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = foreman.ID, ResourceID = 19, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = foreman.ID, ResourceID = 20, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = foreman.ID, ResourceID = 21, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = foreman.ID, ResourceID = 22, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = foreman.ID, ResourceID = 23, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = foreman.ID, ResourceID = 24, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = foreman.ID, ResourceID = 25, Read = true, Write = true, Delete = false },
                new RoleResource() { RoleID = foreman.ID, ResourceID = 26, Read = false, Write = false, Delete = false },
                new RoleResource() { RoleID = foreman.ID, ResourceID = 27, Read = false, Write = false, Delete = false },
                new RoleResource() { RoleID = foreman.ID, ResourceID = 28, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = foreman.ID, ResourceID = 29, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = foreman.ID, ResourceID = 30, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = foreman.ID, ResourceID = 31, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = foreman.ID, ResourceID = 32, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = foreman.ID, ResourceID = 34, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = foreman.ID, ResourceID = 35, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = foreman.ID, ResourceID = 36, Read = false, Write = true, Delete = false },
                new RoleResource() { RoleID = foreman.ID, ResourceID = 37, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = foreman.ID, ResourceID = 38, Read = false, Write = false, Delete = false },
                new RoleResource() { RoleID = foreman.ID, ResourceID = 39, Read = false, Write = false, Delete = false },
                new RoleResource() { RoleID = foreman.ID, ResourceID = 40, Read = false, Write = false, Delete = false }
            );

            context.RoleResources.AddOrUpdate(
                new RoleResource() { RoleID = truckingReports.ID, ResourceID = 1, Read = false, Write = false, Delete = false },
                new RoleResource() { RoleID = truckingReports.ID, ResourceID = 2, Read = false, Write = false, Delete = false },
                new RoleResource() { RoleID = truckingReports.ID, ResourceID = 3, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = truckingReports.ID, ResourceID = 4, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = truckingReports.ID, ResourceID = 5, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = truckingReports.ID, ResourceID = 6, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = truckingReports.ID, ResourceID = 7, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = truckingReports.ID, ResourceID = 8, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = truckingReports.ID, ResourceID = 9, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = truckingReports.ID, ResourceID = 10, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = truckingReports.ID, ResourceID = 11, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = truckingReports.ID, ResourceID = 12, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = truckingReports.ID, ResourceID = 13, Read = true, Write = false, Delete = true },
                new RoleResource() { RoleID = truckingReports.ID, ResourceID = 14, Read = true, Write = false, Delete = true },
                new RoleResource() { RoleID = truckingReports.ID, ResourceID = 15, Read = true, Write = false, Delete = true },
                new RoleResource() { RoleID = truckingReports.ID, ResourceID = 16, Read = true, Write = false, Delete = true },
                new RoleResource() { RoleID = truckingReports.ID, ResourceID = 17, Read = true, Write = false, Delete = true },
                new RoleResource() { RoleID = truckingReports.ID, ResourceID = 18, Read = true, Write = false, Delete = true },
                new RoleResource() { RoleID = truckingReports.ID, ResourceID = 19, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = truckingReports.ID, ResourceID = 20, Read = true, Write = false, Delete = true },
                new RoleResource() { RoleID = truckingReports.ID, ResourceID = 21, Read = true, Write = false, Delete = true },
                new RoleResource() { RoleID = truckingReports.ID, ResourceID = 22, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = truckingReports.ID, ResourceID = 23, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = truckingReports.ID, ResourceID = 24, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = truckingReports.ID, ResourceID = 25, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = truckingReports.ID, ResourceID = 26, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = truckingReports.ID, ResourceID = 27, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = truckingReports.ID, ResourceID = 28, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = truckingReports.ID, ResourceID = 29, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = truckingReports.ID, ResourceID = 30, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = truckingReports.ID, ResourceID = 31, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = truckingReports.ID, ResourceID = 32, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = truckingReports.ID, ResourceID = 34, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = truckingReports.ID, ResourceID = 35, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = truckingReports.ID, ResourceID = 36, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = truckingReports.ID, ResourceID = 37, Read = true, Write = false, Delete = true },
                new RoleResource() { RoleID = truckingReports.ID, ResourceID = 38, Read = false, Write = false, Delete = false },
                new RoleResource() { RoleID = truckingReports.ID, ResourceID = 39, Read = false, Write = false, Delete = false },
                new RoleResource() { RoleID = truckingReports.ID, ResourceID = 40, Read = false, Write = false, Delete = false }
            );

            context.RoleResources.AddOrUpdate(
                new RoleResource() { RoleID = billing.ID, ResourceID = 1, Read = false, Write = false, Delete = false },
                new RoleResource() { RoleID = billing.ID, ResourceID = 2, Read = false, Write = false, Delete = false },
                new RoleResource() { RoleID = billing.ID, ResourceID = 3, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = billing.ID, ResourceID = 4, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = billing.ID, ResourceID = 5, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = billing.ID, ResourceID = 6, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = billing.ID, ResourceID = 7, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = billing.ID, ResourceID = 8, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = billing.ID, ResourceID = 9, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = billing.ID, ResourceID = 10, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = billing.ID, ResourceID = 11, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = billing.ID, ResourceID = 12, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = billing.ID, ResourceID = 13, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = billing.ID, ResourceID = 14, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = billing.ID, ResourceID = 15, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = billing.ID, ResourceID = 16, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = billing.ID, ResourceID = 17, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = billing.ID, ResourceID = 18, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = billing.ID, ResourceID = 19, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = billing.ID, ResourceID = 20, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = billing.ID, ResourceID = 21, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = billing.ID, ResourceID = 22, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = billing.ID, ResourceID = 23, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = billing.ID, ResourceID = 24, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = billing.ID, ResourceID = 25, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = billing.ID, ResourceID = 26, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = billing.ID, ResourceID = 27, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = billing.ID, ResourceID = 28, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = billing.ID, ResourceID = 29, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = billing.ID, ResourceID = 30, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = billing.ID, ResourceID = 31, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = billing.ID, ResourceID = 32, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = billing.ID, ResourceID = 34, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = billing.ID, ResourceID = 35, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = billing.ID, ResourceID = 36, Read = true, Write = true, Delete = false },
                new RoleResource() { RoleID = billing.ID, ResourceID = 37, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = billing.ID, ResourceID = 38, Read = false, Write = false, Delete = false },
                new RoleResource() { RoleID = billing.ID, ResourceID = 39, Read = false, Write = false, Delete = false },
                new RoleResource() { RoleID = billing.ID, ResourceID = 40, Read = false, Write = false, Delete = false }
            );

            context.RoleResources.AddOrUpdate(
                new RoleResource() { RoleID = HR.ID, ResourceID = 1, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = HR.ID, ResourceID = 2, Read = false, Write = false, Delete = false },
                new RoleResource() { RoleID = HR.ID, ResourceID = 3, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = HR.ID, ResourceID = 4, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = HR.ID, ResourceID = 5, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = HR.ID, ResourceID = 6, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = HR.ID, ResourceID = 7, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = HR.ID, ResourceID = 8, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = HR.ID, ResourceID = 9, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = HR.ID, ResourceID = 10, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = HR.ID, ResourceID = 11, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = HR.ID, ResourceID = 12, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = HR.ID, ResourceID = 13, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = HR.ID, ResourceID = 14, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = HR.ID, ResourceID = 15, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = HR.ID, ResourceID = 16, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = HR.ID, ResourceID = 17, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = HR.ID, ResourceID = 18, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = HR.ID, ResourceID = 19, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = HR.ID, ResourceID = 20, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = HR.ID, ResourceID = 21, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = HR.ID, ResourceID = 22, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = HR.ID, ResourceID = 23, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = HR.ID, ResourceID = 24, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = HR.ID, ResourceID = 25, Read = true, Write = true, Delete = false },
                new RoleResource() { RoleID = HR.ID, ResourceID = 26, Read = false, Write = false, Delete = false },
                new RoleResource() { RoleID = HR.ID, ResourceID = 27, Read = false, Write = false, Delete = false },
                new RoleResource() { RoleID = HR.ID, ResourceID = 28, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = HR.ID, ResourceID = 29, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = HR.ID, ResourceID = 30, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = HR.ID, ResourceID = 31, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = HR.ID, ResourceID = 32, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = HR.ID, ResourceID = 34, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = HR.ID, ResourceID = 35, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = HR.ID, ResourceID = 36, Read = false, Write = true, Delete = false },
                new RoleResource() { RoleID = HR.ID, ResourceID = 37, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = HR.ID, ResourceID = 38, Read = false, Write = false, Delete = false },
                new RoleResource() { RoleID = HR.ID, ResourceID = 39, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = HR.ID, ResourceID = 40, Read = true, Write = false, Delete = false }
            );

            context.RoleResources.AddOrUpdate(
                new RoleResource() { RoleID = truckingAdmin.ID, ResourceID = 1, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = truckingAdmin.ID, ResourceID = 2, Read = false, Write = false, Delete = false },
                new RoleResource() { RoleID = truckingAdmin.ID, ResourceID = 3, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = truckingAdmin.ID, ResourceID = 4, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = truckingAdmin.ID, ResourceID = 5, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = truckingAdmin.ID, ResourceID = 6, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = truckingAdmin.ID, ResourceID = 7, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = truckingAdmin.ID, ResourceID = 8, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = truckingAdmin.ID, ResourceID = 9, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = truckingAdmin.ID, ResourceID = 10, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = truckingAdmin.ID, ResourceID = 11, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = truckingAdmin.ID, ResourceID = 12, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = truckingAdmin.ID, ResourceID = 13, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = truckingAdmin.ID, ResourceID = 14, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = truckingAdmin.ID, ResourceID = 15, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = truckingAdmin.ID, ResourceID = 16, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = truckingAdmin.ID, ResourceID = 17, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = truckingAdmin.ID, ResourceID = 18, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = truckingAdmin.ID, ResourceID = 19, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = truckingAdmin.ID, ResourceID = 20, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = truckingAdmin.ID, ResourceID = 21, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = truckingAdmin.ID, ResourceID = 22, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = truckingAdmin.ID, ResourceID = 23, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = truckingAdmin.ID, ResourceID = 24, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = truckingAdmin.ID, ResourceID = 25, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = truckingAdmin.ID, ResourceID = 26, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = truckingAdmin.ID, ResourceID = 27, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = truckingAdmin.ID, ResourceID = 28, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = truckingAdmin.ID, ResourceID = 29, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = truckingAdmin.ID, ResourceID = 30, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = truckingAdmin.ID, ResourceID = 31, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = truckingAdmin.ID, ResourceID = 32, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = truckingAdmin.ID, ResourceID = 34, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = truckingAdmin.ID, ResourceID = 35, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = truckingAdmin.ID, ResourceID = 36, Read = true, Write = true, Delete = false },
                new RoleResource() { RoleID = truckingAdmin.ID, ResourceID = 37, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = truckingAdmin.ID, ResourceID = 38, Read = false, Write = false, Delete = false },
                new RoleResource() { RoleID = truckingAdmin.ID, ResourceID = 39, Read = false, Write = false, Delete = false },
                new RoleResource() { RoleID = truckingAdmin.ID, ResourceID = 40, Read = false, Write = false, Delete = false }
            );

            context.RoleResources.AddOrUpdate(
                new RoleResource() { RoleID = billingAdmin.ID, ResourceID = 1, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = billingAdmin.ID, ResourceID = 2, Read = false, Write = false, Delete = false },
                new RoleResource() { RoleID = billingAdmin.ID, ResourceID = 3, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = billingAdmin.ID, ResourceID = 4, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = billingAdmin.ID, ResourceID = 5, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = billingAdmin.ID, ResourceID = 6, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = billingAdmin.ID, ResourceID = 7, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = billingAdmin.ID, ResourceID = 8, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = billingAdmin.ID, ResourceID = 9, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = billingAdmin.ID, ResourceID = 10, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = billingAdmin.ID, ResourceID = 11, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = billingAdmin.ID, ResourceID = 12, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = billingAdmin.ID, ResourceID = 13, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = billingAdmin.ID, ResourceID = 14, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = billingAdmin.ID, ResourceID = 15, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = billingAdmin.ID, ResourceID = 16, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = billingAdmin.ID, ResourceID = 17, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = billingAdmin.ID, ResourceID = 18, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = billingAdmin.ID, ResourceID = 19, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = billingAdmin.ID, ResourceID = 20, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = billingAdmin.ID, ResourceID = 21, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = billingAdmin.ID, ResourceID = 22, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = billingAdmin.ID, ResourceID = 23, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = billingAdmin.ID, ResourceID = 24, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = billingAdmin.ID, ResourceID = 25, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = billingAdmin.ID, ResourceID = 26, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = billingAdmin.ID, ResourceID = 27, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = billingAdmin.ID, ResourceID = 28, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = billingAdmin.ID, ResourceID = 29, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = billingAdmin.ID, ResourceID = 30, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = billingAdmin.ID, ResourceID = 31, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = billingAdmin.ID, ResourceID = 32, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = billingAdmin.ID, ResourceID = 34, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = billingAdmin.ID, ResourceID = 35, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = billingAdmin.ID, ResourceID = 36, Read = true, Write = true, Delete = false },
                new RoleResource() { RoleID = billingAdmin.ID, ResourceID = 37, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = billingAdmin.ID, ResourceID = 38, Read = false, Write = false, Delete = false },
                new RoleResource() { RoleID = billingAdmin.ID, ResourceID = 39, Read = false, Write = false, Delete = false },
                new RoleResource() { RoleID = billingAdmin.ID, ResourceID = 40, Read = false, Write = false, Delete = false }
            );

            context.RoleResources.AddOrUpdate(
                new RoleResource() { RoleID = mobileUser.ID, ResourceID = 1, Read = false, Write = false, Delete = false },
                new RoleResource() { RoleID = mobileUser.ID, ResourceID = 2, Read = false, Write = false, Delete = false },
                new RoleResource() { RoleID = mobileUser.ID, ResourceID = 3, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = mobileUser.ID, ResourceID = 4, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = mobileUser.ID, ResourceID = 5, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = mobileUser.ID, ResourceID = 6, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = mobileUser.ID, ResourceID = 7, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = mobileUser.ID, ResourceID = 8, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = mobileUser.ID, ResourceID = 9, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = mobileUser.ID, ResourceID = 10, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = mobileUser.ID, ResourceID = 11, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = mobileUser.ID, ResourceID = 12, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = mobileUser.ID, ResourceID = 13, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = mobileUser.ID, ResourceID = 14, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = mobileUser.ID, ResourceID = 15, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = mobileUser.ID, ResourceID = 16, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = mobileUser.ID, ResourceID = 17, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = mobileUser.ID, ResourceID = 18, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = mobileUser.ID, ResourceID = 19, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = mobileUser.ID, ResourceID = 20, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = mobileUser.ID, ResourceID = 21, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = mobileUser.ID, ResourceID = 22, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = mobileUser.ID, ResourceID = 23, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = mobileUser.ID, ResourceID = 24, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = mobileUser.ID, ResourceID = 25, Read = true, Write = true, Delete = false },
                new RoleResource() { RoleID = mobileUser.ID, ResourceID = 26, Read = false, Write = false, Delete = false },
                new RoleResource() { RoleID = mobileUser.ID, ResourceID = 27, Read = false, Write = false, Delete = false },
                new RoleResource() { RoleID = mobileUser.ID, ResourceID = 28, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = mobileUser.ID, ResourceID = 29, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = mobileUser.ID, ResourceID = 30, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = mobileUser.ID, ResourceID = 31, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = mobileUser.ID, ResourceID = 32, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = mobileUser.ID, ResourceID = 34, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = mobileUser.ID, ResourceID = 35, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = mobileUser.ID, ResourceID = 36, Read = false, Write = true, Delete = false },
                new RoleResource() { RoleID = mobileUser.ID, ResourceID = 37, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = mobileUser.ID, ResourceID = 38, Read = false, Write = false, Delete = false },
                new RoleResource() { RoleID = mobileUser.ID, ResourceID = 39, Read = false, Write = false, Delete = false },
                new RoleResource() { RoleID = mobileUser.ID, ResourceID = 40, Read = false, Write = false, Delete = false }
            );

            context.RoleResources.AddOrUpdate(
                new RoleResource() { RoleID = manager.ID, ResourceID = 1, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = manager.ID, ResourceID = 2, Read = false, Write = false, Delete = false },
                new RoleResource() { RoleID = manager.ID, ResourceID = 3, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = manager.ID, ResourceID = 4, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = manager.ID, ResourceID = 5, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = manager.ID, ResourceID = 6, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = manager.ID, ResourceID = 7, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = manager.ID, ResourceID = 8, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = manager.ID, ResourceID = 9, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = manager.ID, ResourceID = 10, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = manager.ID, ResourceID = 11, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = manager.ID, ResourceID = 12, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = manager.ID, ResourceID = 13, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = manager.ID, ResourceID = 14, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = manager.ID, ResourceID = 15, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = manager.ID, ResourceID = 16, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = manager.ID, ResourceID = 17, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = manager.ID, ResourceID = 18, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = manager.ID, ResourceID = 19, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = manager.ID, ResourceID = 20, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = manager.ID, ResourceID = 21, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = manager.ID, ResourceID = 22, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = manager.ID, ResourceID = 23, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = manager.ID, ResourceID = 24, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = manager.ID, ResourceID = 25, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = manager.ID, ResourceID = 26, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = manager.ID, ResourceID = 27, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = manager.ID, ResourceID = 28, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = manager.ID, ResourceID = 29, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = manager.ID, ResourceID = 30, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = manager.ID, ResourceID = 31, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = manager.ID, ResourceID = 32, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = manager.ID, ResourceID = 34, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = manager.ID, ResourceID = 35, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = manager.ID, ResourceID = 36, Read = true, Write = true, Delete = false },
                new RoleResource() { RoleID = manager.ID, ResourceID = 37, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = manager.ID, ResourceID = 38, Read = false, Write = false, Delete = false },
                new RoleResource() { RoleID = manager.ID, ResourceID = 39, Read = false, Write = false, Delete = false },
                new RoleResource() { RoleID = manager.ID, ResourceID = 40, Read = false, Write = false, Delete = false }
            );
            context.RoleResources.AddOrUpdate(
                new RoleResource() { RoleID = maintenanceAdmin.ID, ResourceID = 1, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = maintenanceAdmin.ID, ResourceID = 2, Read = false, Write = false, Delete = false },
                new RoleResource() { RoleID = maintenanceAdmin.ID, ResourceID = 3, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = maintenanceAdmin.ID, ResourceID = 4, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = maintenanceAdmin.ID, ResourceID = 5, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = maintenanceAdmin.ID, ResourceID = 6, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = maintenanceAdmin.ID, ResourceID = 7, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = maintenanceAdmin.ID, ResourceID = 8, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = maintenanceAdmin.ID, ResourceID = 9, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = maintenanceAdmin.ID, ResourceID = 10, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = maintenanceAdmin.ID, ResourceID = 11, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = maintenanceAdmin.ID, ResourceID = 12, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = maintenanceAdmin.ID, ResourceID = 13, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = maintenanceAdmin.ID, ResourceID = 14, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = maintenanceAdmin.ID, ResourceID = 15, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = maintenanceAdmin.ID, ResourceID = 16, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = maintenanceAdmin.ID, ResourceID = 17, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = maintenanceAdmin.ID, ResourceID = 18, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = maintenanceAdmin.ID, ResourceID = 19, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = maintenanceAdmin.ID, ResourceID = 20, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = maintenanceAdmin.ID, ResourceID = 21, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = maintenanceAdmin.ID, ResourceID = 22, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = maintenanceAdmin.ID, ResourceID = 23, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = maintenanceAdmin.ID, ResourceID = 24, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = maintenanceAdmin.ID, ResourceID = 25, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = maintenanceAdmin.ID, ResourceID = 26, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = maintenanceAdmin.ID, ResourceID = 27, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = maintenanceAdmin.ID, ResourceID = 28, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = maintenanceAdmin.ID, ResourceID = 29, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = maintenanceAdmin.ID, ResourceID = 30, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = maintenanceAdmin.ID, ResourceID = 31, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = maintenanceAdmin.ID, ResourceID = 32, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = maintenanceAdmin.ID, ResourceID = 34, Read = true, Write = false, Delete = false },
                new RoleResource() { RoleID = maintenanceAdmin.ID, ResourceID = 35, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = maintenanceAdmin.ID, ResourceID = 36, Read = true, Write = true, Delete = false },
                new RoleResource() { RoleID = maintenanceAdmin.ID, ResourceID = 37, Read = true, Write = true, Delete = true },
                new RoleResource() { RoleID = maintenanceAdmin.ID, ResourceID = 38, Read = false, Write = false, Delete = false },
                new RoleResource() { RoleID = maintenanceAdmin.ID, ResourceID = 39, Read = false, Write = false, Delete = false },
                new RoleResource() { RoleID = maintenanceAdmin.ID, ResourceID = 40, Read = false, Write = false, Delete = false }
            );

            #endregion RoleResources

            #region AuthSection

            var authSectionEnums = (AuthSectionID[])Enum.GetValues(typeof(AuthSectionID));
            var authSections = authSectionEnums.Select(x => new AuthSection() { ID = x, Name = x.ToString(), Description = x.ToString() }).ToArray();
            context.AuthSections.AddOrUpdate(x => x.ID, authSections);

            context.SaveChanges();

            #endregion AuthSection

            #region AuthActivity

            var authActivityEnums = (AuthActivityID[])Enum.GetValues(typeof(AuthActivityID));

            var allAuthActivities = new List<AuthActivity>()
                {
                    // Permissions
                    new AuthActivity() { ID = (int)AuthActivityID.EditPermissions, Name = "Edit Permissions", AuthSectionID = AuthSectionID.Unknown, Description = "" },
                    
                    // Home
                    new AuthActivity() { ID = (int)AuthActivityID.ClockInFromWeb, Name = "Clock in on the website", AuthSectionID = AuthSectionID.Unknown, Description = "" },
                    new AuthActivity() { ID = (int)AuthActivityID.ViewTimeCard, Name = "View your own punch history", AuthSectionID = AuthSectionID.Unknown, Description = "" },
                    
                    // Timers
                    new AuthActivity() { ID = (int)AuthActivityID.ViewAccountingTimers, Name = "View accounting timer page", AuthSectionID = AuthSectionID.Unknown, Description = "" },
                    new AuthActivity() { ID = (int)AuthActivityID.ViewBillingTimers, Name = "View billing timer page", AuthSectionID = AuthSectionID.Unknown, Description = "" },
                    new AuthActivity() { ID = (int)AuthActivityID.ViewSupervisorTimers, Name = "View supervisor timer page", AuthSectionID = AuthSectionID.Unknown, Description = "" },
                    new AuthActivity() { ID = (int)AuthActivityID.ViewForemenTimesheets, Name = "View foremen timesheets page", AuthSectionID = AuthSectionID.Unknown, Description = "" },
                    new AuthActivity() { ID = (int)AuthActivityID.ViewEmployeeTimesheets, Name = "View employee timesheets page", AuthSectionID = AuthSectionID.Unknown, Description = "" },
                    new AuthActivity() { ID = (int)AuthActivityID.RejectBillingApproval, Name = "Reject Billing's timer approval", AuthSectionID = AuthSectionID.Employee, Description = "" },
                    new AuthActivity() { ID = (int)AuthActivityID.RejectAccountingApproval, Name = "Reject Accounting's timer approval", AuthSectionID = AuthSectionID.Employee, Description = "" },
                    new AuthActivity() { ID = (int)AuthActivityID.RejectSupervisorApproval, Name = "Reject Supervisor's timer approval", AuthSectionID = AuthSectionID.Employee, Description = "" },
                    new AuthActivity() { ID = (int)AuthActivityID.ApproveAsAccounting, Name = "Approve timers from Accounting's queue", AuthSectionID = AuthSectionID.Employee, Description = "" },
                    new AuthActivity() { ID = (int)AuthActivityID.ApproveAsBilling, Name = "Approve timers from Billing's queue", AuthSectionID = AuthSectionID.Employee, Description = "" },
                    new AuthActivity() { ID = (int)AuthActivityID.ApproveAsSupervisor, Name = "Approve timers from a Supervisor's queue", AuthSectionID = AuthSectionID.Employee, Description = "" },             

                    // Employee Timers
                    new AuthActivity() { ID = (int)AuthActivityID.AddEmployeeTimer, Name = "Add employee timesheet", AuthSectionID = AuthSectionID.Unknown, Description = "" },
                    new AuthActivity() { ID = (int)AuthActivityID.ViewEmployeeTimer, Name = "View employee timesheet", AuthSectionID = AuthSectionID.Unknown, Description = "" },
                    new AuthActivity() { ID = (int)AuthActivityID.ApproveEmployeeTimer, Name = "Approve employee timesheet", AuthSectionID = AuthSectionID.Unknown, Description = "" },
                    new AuthActivity() { ID = (int)AuthActivityID.FlagEmployeeTimer, Name = "Flag an employee timesheet", AuthSectionID = AuthSectionID.Unknown, Description = "" },

                    // Employee Timesheets
                    new AuthActivity() { ID = (int)AuthActivityID.EditEmployeeTimerEntry, Name = "Edit employee timer entry", AuthSectionID = AuthSectionID.Unknown, Description = "" },
                    new AuthActivity() { ID = (int)AuthActivityID.DeleteEmployeeTimerEntry, Name = "Delete employee timer entry", AuthSectionID = AuthSectionID.Unknown, Description = "" },
                    new AuthActivity() { ID = (int)AuthActivityID.AddEmployeeTimerEntry, Name = "Add employee timer entry", AuthSectionID = AuthSectionID.Unknown, Description = "" },
                    new AuthActivity() { ID = (int)AuthActivityID.ViewEmployeeTimerEditHistory, Name = "View employee timer edit history", AuthSectionID = AuthSectionID.Unknown, Description = "" },
                    new AuthActivity() { ID = (int)AuthActivityID.ViewEmployeeOccurrences, Name = "View employee timesheet occurrences", AuthSectionID = AuthSectionID.Unknown, Description = "" },
                    new AuthActivity() { ID = (int)AuthActivityID.EditEmployeeOccurrences, Name = "Edit employee timesheet occurrences", AuthSectionID = AuthSectionID.Unknown, Description = "" },
                    new AuthActivity() { ID = (int)AuthActivityID.DeleteEmployeeOccurrences, Name = "Delete employee timesheet occurrences", AuthSectionID = AuthSectionID.Unknown, Description = "" },
                    new AuthActivity() { ID = (int)AuthActivityID.AddTimersFromSupervisorCard, Name = "Add timers from a supervisor card", AuthSectionID = AuthSectionID.Unknown, Description = "" },
                    new AuthActivity() { ID = (int)AuthActivityID.EditTimerOverhead, Name = "Edit timesheet overhead", AuthSectionID = AuthSectionID.Unknown, Description = "" },
                    new AuthActivity() { ID = (int)AuthActivityID.ViewTimerOverheadEditHistory, Name = "View timesheet overhead edit history", AuthSectionID = AuthSectionID.Unknown, Description = "" },
                    new AuthActivity() { ID = (int)AuthActivityID.ApproveEmployeeTimesheet, Name = "Approve employee timesheet", AuthSectionID = AuthSectionID.Unknown, Description = "" },
                    new AuthActivity() { ID = (int)AuthActivityID.RejectEmployeeTimesheet, Name = "Reject employee timesheet", AuthSectionID = AuthSectionID.Unknown, Description = "" },
                    new AuthActivity() { ID = (int)AuthActivityID.EditOdometerReading, Name = "Edit odometer reading", AuthSectionID = AuthSectionID.Unknown, Description = "" },
                    new AuthActivity() { ID = (int)AuthActivityID.ResolveEmployeeTimerFlag, Name = "Resolve employee timesheet flag", AuthSectionID = AuthSectionID.Unknown, Description = "" },
                    new AuthActivity() { ID = (int)AuthActivityID.MarkTimerInjury, Name = "Mark employee as injured on timesheet", AuthSectionID = AuthSectionID.Unknown, Description = "" },
                    new AuthActivity() { ID = (int)AuthActivityID.EditTimerNote, Name = "Edit a timer note", AuthSectionID = AuthSectionID.Unknown, Description = "" },

                    // Foremen Timesheets
                    new AuthActivity() { ID = (int)AuthActivityID.AddForemanTimesheet, Name = "Add a foreman timesheet", AuthSectionID = AuthSectionID.Unknown, Description = "" },
                    new AuthActivity() { ID = (int)AuthActivityID.ViewForemanTimesheet, Name = "View a foreman timesheet", AuthSectionID = AuthSectionID.Unknown, Description = "" },
                    new AuthActivity() { ID = (int)AuthActivityID.EditForemanTimer, Name = "Edit a foreman timesheet", AuthSectionID = AuthSectionID.Unknown, Description = "" },
                    new AuthActivity() { ID = (int)AuthActivityID.ViewForemanTimeCard, Name = "View a foreman timecard", AuthSectionID = AuthSectionID.Unknown, Description = "" },
                    new AuthActivity() { ID = (int)AuthActivityID.ApproveForemanTimer, Name = "Approve a foreman timecard", AuthSectionID = AuthSectionID.Unknown, Description = "" },
                    new AuthActivity() { ID = (int)AuthActivityID.FlagForemanTimer, Name = "Flag a foreman timesheet", AuthSectionID = AuthSectionID.Unknown, Description = "" },
                    new AuthActivity() { ID = (int)AuthActivityID.RejectForemanTimer, Name = "Reject a foreman timesheet", AuthSectionID = AuthSectionID.Unknown, Description = "" },
                    new AuthActivity() { ID = (int)AuthActivityID.EditForemanTimerJob, Name = "Edit a foreman job timer", AuthSectionID = AuthSectionID.Unknown, Description = "" },
                    new AuthActivity() { ID = (int)AuthActivityID.DeleteForemanTimerJob, Name = "Delete a foreman job timer", AuthSectionID = AuthSectionID.Unknown, Description = "" },
                    new AuthActivity() { ID = (int)AuthActivityID.AddForemanTimerJob, Name = "Add a foreman job timer", AuthSectionID = AuthSectionID.Unknown, Description = "" },
                    new AuthActivity() { ID = (int)AuthActivityID.AddForemanTimerJobEquipment, Name = "Add an equipment job timer to a foreman timecard", AuthSectionID = AuthSectionID.Unknown, Description = "" },
                    new AuthActivity() { ID = (int)AuthActivityID.DeleteForemanTimerJobEquipment, Name = "Delete an equipment job timer for a foreman timecard", AuthSectionID = AuthSectionID.Unknown, Description = "" },
                    new AuthActivity() { ID = (int)AuthActivityID.AddForemanTimerOccurrence, Name = "Add an occurrence to a foreman timesheet", AuthSectionID = AuthSectionID.Unknown, Description = "" },
                    new AuthActivity() { ID = (int)AuthActivityID.EditForemanTimerOccurrence, Name = "Edit an occurrence to a foreman timesheet", AuthSectionID = AuthSectionID.Unknown, Description = "" },
                    new AuthActivity() { ID = (int)AuthActivityID.DeleteForemanTimerOccurrence, Name = "Delete an occurrence from a foreman timesheet", AuthSectionID = AuthSectionID.Unknown, Description = "" },

                    // Trucking
                    new AuthActivity() { ID = (int)AuthActivityID.ViewTruckerDailies, Name = "View trucker dailies page", AuthSectionID = AuthSectionID.Unknown, Description = "" },
                    new AuthActivity() { ID = (int)AuthActivityID.EditTruckerDaily, Name = "Edit a trucker daily row", AuthSectionID = AuthSectionID.Unknown, Description = "" },
                    new AuthActivity() { ID = (int)AuthActivityID.ExportTruckerDailiesAsCsv, Name = "Export the trucker dailies as a CSV", AuthSectionID = AuthSectionID.Unknown, Description = "" },
                    new AuthActivity() { ID = (int)AuthActivityID.ExportVisibleTruckerDailiesAsCsv, Name = "Export the visible trucker dailies as a CSV", AuthSectionID = AuthSectionID.Unknown, Description = "" },
                    new AuthActivity() { ID = (int)AuthActivityID.ViewTruckingPricing, Name = "View trucking pricing page", AuthSectionID = AuthSectionID.Unknown, Description = "" },
                    new AuthActivity() { ID = (int)AuthActivityID.ViewTruckingReporting, Name = "View trucking reporting page", AuthSectionID = AuthSectionID.Unknown, Description = "" },

                    // TM Crushing
                    new AuthActivity() { ID = (int)AuthActivityID.ViewTMCrushing, Name = "View TM Crushing page", AuthSectionID = AuthSectionID.Unknown, Description = "" },
                    new AuthActivity() { ID = (int)AuthActivityID.EditProductionItem, Name = "Edit a TM Crushing production item", AuthSectionID = AuthSectionID.Unknown, Description = "" },
                    new AuthActivity() { ID = (int)AuthActivityID.AddProductionItem, Name = "Add a TM Crushing production item", AuthSectionID = AuthSectionID.Unknown, Description = "" },
                    new AuthActivity() { ID = (int)AuthActivityID.DeleteProductionItem, Name = "Delete a TM Crushing production item", AuthSectionID = AuthSectionID.Unknown, Description = "" },
                    new AuthActivity() { ID = (int)AuthActivityID.EditDowntimeItem, Name = "Edit a TM Crushing downtime item", AuthSectionID = AuthSectionID.Unknown, Description = "" },
                    new AuthActivity() { ID = (int)AuthActivityID.AddDowntimeItem, Name = "Add a TM Crushing downtime item", AuthSectionID = AuthSectionID.Unknown, Description = "" },
                    new AuthActivity() { ID = (int)AuthActivityID.DeleteDowntimeItem, Name = "Delete a TM Crushing downtime item", AuthSectionID = AuthSectionID.Unknown, Description = "" },
                    new AuthActivity() { ID = (int)AuthActivityID.AddCrushingReportNotes, Name = "Add TM Crushing report notes", AuthSectionID = AuthSectionID.Unknown, Description = "" },
                    new AuthActivity() { ID = (int)AuthActivityID.SubmitCrushingReport, Name = "Submit TM Crushing report", AuthSectionID = AuthSectionID.Unknown, Description = "" },

                    // Admin
                    new AuthActivity() { ID = (int)AuthActivityID.ViewDowntimeReasons, Name = "View downtime reasons page", AuthSectionID = AuthSectionID.Unknown, Description = "" },
                    new AuthActivity() { ID = (int)AuthActivityID.EditDowntimeReasons, Name = "Edit a downtime reason", AuthSectionID = AuthSectionID.Unknown, Description = "" },
                    new AuthActivity() { ID = (int)AuthActivityID.DeleteDowntimeReason, Name = "Delete a downtime reason", AuthSectionID = AuthSectionID.Unknown, Description = "" },
                    new AuthActivity() { ID = (int)AuthActivityID.CreateDowntimeReason, Name = "Create a downtime reason", AuthSectionID = AuthSectionID.Unknown, Description = "" },

                    new AuthActivity() { ID = (int)AuthActivityID.ViewGpsSettings, Name = "View the GPS settings page", AuthSectionID = AuthSectionID.Unknown, Description = "" },
                    new AuthActivity() { ID = (int)AuthActivityID.ViewJobs, Name = "View the jobs page", AuthSectionID = AuthSectionID.Unknown, Description = "" },
                    new AuthActivity() { ID = (int)AuthActivityID.ViewMaterials, Name = "View the materials page", AuthSectionID = AuthSectionID.Unknown, Description = "" },
                    new AuthActivity() { ID = (int)AuthActivityID.ViewOccurrences, Name = "View employee occurrences page", AuthSectionID = AuthSectionID.Unknown, Description = "" },
                    new AuthActivity() { ID = (int)AuthActivityID.ViewTruckingClassifications, Name = "View trucking classifications page", AuthSectionID = AuthSectionID.Unknown, Description = "" },

                    new AuthActivity() { ID = (int)AuthActivityID.EditMaterial, Name = "Edit a material", AuthSectionID = AuthSectionID.Unknown, Description = "" },
                    new AuthActivity() { ID = (int)AuthActivityID.DeleteMaterial, Name = "Delete a material", AuthSectionID = AuthSectionID.Unknown, Description = "" },
                    new AuthActivity() { ID = (int)AuthActivityID.AddMaterial, Name = "Add a material", AuthSectionID = AuthSectionID.Unknown, Description = "" },

                    new AuthActivity() { ID = (int)AuthActivityID.EditOccurrence, Name = "Edit an occurrence", AuthSectionID = AuthSectionID.Unknown, Description = "" },
                    new AuthActivity() { ID = (int)AuthActivityID.DeleteOccurrence, Name = "Delete an occurrence", AuthSectionID = AuthSectionID.Unknown, Description = "" },
                    new AuthActivity() { ID = (int)AuthActivityID.AddOccurrence, Name = "Add an occurrence", AuthSectionID = AuthSectionID.Unknown, Description = "" },

                    // HR
                    new AuthActivity() { ID = (int)AuthActivityID.ViewEmployeeList, Name = "View the list of employees", AuthSectionID = AuthSectionID.Unknown, Description = "" },
                    new AuthActivity() { ID = (int)AuthActivityID.CreateEmployee, Name = "Create an employee", AuthSectionID = AuthSectionID.Unknown, Description = "" },
                    new AuthActivity() { ID = (int)AuthActivityID.ViewSupervisors, Name = "View current supervisors page", AuthSectionID = AuthSectionID.Unknown, Description = "" },

                    // Employee List/Details
                    new AuthActivity() { ID = (int)AuthActivityID.SearchEmployees, Name = "Search Employees from the header", AuthSectionID = AuthSectionID.Unknown, Description = "" },
                    new AuthActivity() { ID = (int)AuthActivityID.ViewEmployeeDetails, Name = "View employee details", AuthSectionID = AuthSectionID.Unknown, Description = "" },

                    new AuthActivity() { ID = (int)AuthActivityID.AddEmployeeRole, Name = "Add a role to an employee", AuthSectionID = AuthSectionID.Employee, Description = "" },
                    new AuthActivity() { ID = (int)AuthActivityID.DeleteEmployeeRole, Name = "Delete a role from an employee", AuthSectionID = AuthSectionID.Employee, Description = "" },
                    new AuthActivity() { ID = (int)AuthActivityID.AddEmployeeDepartment, Name = "Add a department to an employee", AuthSectionID = AuthSectionID.Employee, Description = "" },
                    new AuthActivity() { ID = (int)AuthActivityID.DeleteEmployeeDepartment, Name = "Delete a department from an employee", AuthSectionID = AuthSectionID.Employee, Description = "" },
                    new AuthActivity() { ID = (int)AuthActivityID.AddEmployeeSupervisor, Name = "Add a supervisor for an employee", AuthSectionID = AuthSectionID.Employee, Description = "" },
                    new AuthActivity() { ID = (int)AuthActivityID.DeleteEmployeeSupervisor, Name = "Remove a supervisor from an employee", AuthSectionID = AuthSectionID.Employee, Description = "" },
                    new AuthActivity() { ID = (int)AuthActivityID.ChangeEmployeePassword, Name = "Change an employee's password", AuthSectionID = AuthSectionID.Employee, Description = "" },
                    new AuthActivity() { ID = (int)AuthActivityID.ChangeEmployeePin, Name = "Change an employee's pin", AuthSectionID = AuthSectionID.Employee, Description = "" },
                    new AuthActivity() { ID = (int)AuthActivityID.ChangeEmployeeUsername, Name = "Change an employee's username", AuthSectionID = AuthSectionID.Employee, Description = "" },
                    new AuthActivity() { ID = (int)AuthActivityID.ChangeEmployeeName, Name = "Change an employee's name", AuthSectionID = AuthSectionID.Employee, Description = "" },
                    new AuthActivity() { ID = (int)AuthActivityID.ViewCurrentEmployeeDetails, Name = "View the current employee's details", AuthSectionID = AuthSectionID.Employee, Description = "" },
                    new AuthActivity() { ID = (int)AuthActivityID.ChangeEmployeePhone, Name = "Change an employee's phone number", AuthSectionID = AuthSectionID.Unknown, Description = "" },

                    // Csv
                    new AuthActivity() { ID = (int)AuthActivityID.DownloadJobTimersCsv, Name = "Download job timer CSVs", AuthSectionID = AuthSectionID.Unknown, Description = "" },
                    new AuthActivity() { ID = (int)AuthActivityID.DownloadOccurrencesCsv, Name = "Download occurrence CSVs", AuthSectionID = AuthSectionID.Unknown, Description = "" },
                    new AuthActivity() { ID = (int)AuthActivityID.DownloadEmployeeTimecardsCsv, Name = "Download employee timecard CSVs", AuthSectionID = AuthSectionID.Unknown, Description = "" },
                    new AuthActivity() { ID = (int)AuthActivityID.DownloadDiscrepanciesCsv, Name = "Download discrepancy CSVs", AuthSectionID = AuthSectionID.Unknown, Description = "" },
                    new AuthActivity() { ID = (int)AuthActivityID.DownloadLoadTimersCsv, Name = "Download load timer CSVs", AuthSectionID = AuthSectionID.Unknown, Description = "" },
                    new AuthActivity() { ID = (int)AuthActivityID.DownloadDowntimeCsv, Name = "Download downtime CSVs", AuthSectionID = AuthSectionID.Unknown, Description = "" },
                    new AuthActivity() { ID = (int)AuthActivityID.DownloadEquipmentTimersCsv, Name = "Download equipment timer CSVs", AuthSectionID = AuthSectionID.Unknown, Description = "" },
                    new AuthActivity() { ID = (int)AuthActivityID.DownloadJobTimersWithEquipmentCsv, Name = "Download Job Timers With Equipment CSVs", AuthSectionID = AuthSectionID.Unknown, Description = "" },
                    new AuthActivity() { ID = (int)AuthActivityID.DownloadEmployeeRolesCsv, Name = "Download employee roles CSVs", AuthSectionID = AuthSectionID.Unknown, Description = "" },
                    new AuthActivity() { ID = (int)AuthActivityID.DownloadEmployeeClockInsOutsCSV, Name = "Download employee clock ins and outs CSVs", AuthSectionID = AuthSectionID.Unknown, Description = "" },
                    new AuthActivity() { ID = (int)AuthActivityID.DownloadNotesCSV, Name = "Download notes CSVs", AuthSectionID = AuthSectionID.Unknown, Description = "" },
                    new AuthActivity() { ID = (int)AuthActivityID.DownloadQuantitiesCSV, Name = "Download quantities CSVs", AuthSectionID = AuthSectionID.Unknown, Description = "" },
                };



            var authActivities = allAuthActivities.Select(a => { a.Key = ((AuthActivityID)a.ID).ToString(); return a; }).ToArray();

            context.AuthActivities.AddOrUpdate(x => x.ID, authActivities);

            context.SaveChanges();

            #endregion AuthActivity

            #region BillTypes

            var billTypeEnums = (BillTypeName[])Enum.GetValues(typeof(BillTypeName));
            var billTypes = billTypeEnums.Select(x => new BillType() { ID = (int)x, Name = x.ToString() }).ToArray();
            context.BillTypes.AddOrUpdate(x => x.ID, billTypes);

            #endregion BillTypes

            #region EmployeeTypes
            context.EmployeeTypes.AddOrUpdate(x => x.ID,
                    new EmployeeType() { ID = 1, Description = "Part Time" },
                    new EmployeeType() { ID = 2, Description = "Full Time" },
                    new EmployeeType() { ID = 3, Description = "Salaried" }
                );
            #endregion EmployeeTypes

            #region EquipmentServiceTypes
            context.EquipmentServiceTypes.AddOrUpdate(x => x.ID,
                new EquipmentServiceType() { ID = 1, Name = "Repair", Code = "RE" },
                new EquipmentServiceType() { ID = 2, Name = "Service", Code = "SV" },
                new EquipmentServiceType() { ID = 3, Name = "Paint", Code = "PT" },
                new EquipmentServiceType() { ID = 4, Name = "Damage", Code = "DM" },
                new EquipmentServiceType() { ID = 5, Name = "Outfitting", Code = "OF" }
                );
            #endregion EquipmentServiceTypes

            #region Departments

            var departmentEnum = (DepartmentName[])Enum.GetValues(typeof(DepartmentName));
            var departments = departmentEnum.Select(
                x => new Department()
                {
                    ID = (int)x,
                    Name = x.ToString(),
                    AuthenticationTimeoutMinutes = 1080
                }).ToArray();

            foreach (var department in departments)
            {
                var departmentExists = context.Departments.Any(x => x.ID == department.ID && x.Name == department.Name && x.AuthenticationTimeoutMinutes == department.AuthenticationTimeoutMinutes);
                if (!departmentExists)
                {
                    context.Departments.AddOrUpdate(x => x.ID, department);
                }
            }

            #endregion Departments

            #region CustomerTypes

            var customerTypeEnum = (CustomerTypeName[])Enum.GetValues(typeof(CustomerTypeName));
            var customerTypes = customerTypeEnum.Select(x => new CustomerType() { ID = (int)x, Name = x.ToString() }).ToArray();
            context.CustomerTypes.AddOrUpdate(x => x.ID, customerTypes);

            #endregion CustomerTypes

            #region TruckClassifications
            context.TruckClassifications.AddOrUpdate(x => x.ID,
                    new TruckClassification() { ID = 1, Name = "Dump Truck", Truck = "DT", Code = "DT" },
                    new TruckClassification() { ID = 2, Name = "Semi", Truck = "Semi", Code = "S" },
                    new TruckClassification() { ID = 3, Name = "Side Dump", Truck = "Semi", Trailer1 = "SD", Code = "SD" },
                    new TruckClassification() { ID = 4, Name = "Belly Dump", Truck = "Semi", Trailer1 = "BD", Code = "BD" },
                    new TruckClassification() { ID = 5, Name = "Pup Trailer", Truck = "Semi", Trailer1 = "PT", Code = "PT" },
                    new TruckClassification() { ID = 6, Name = "Double Side Dump", Truck = "Semi", Trailer1 = "SD", Trailer2 = "SD", Code = "SD&SD" },
                    new TruckClassification() { ID = 7, Name = "Double Belly Dump", Truck = "Semi", Trailer1 = "BD", Trailer2 = "BD", Code = "BD&BD" },
                    new TruckClassification() { ID = 8, Name = "End Dump", Truck = "Semi", Trailer1 = "ED", Code = "ED" },
                    new TruckClassification() { ID = 9, Name = "Slinger", Truck = "ST", Code = "ST" },
                    new TruckClassification() { ID = 10, Name = "Water Truck", Truck = "WT", Code = "WT" },
                    new TruckClassification() { ID = 11, Name = "Crane Truck", Truck = "CT", Code = "CT" }
                );
            #endregion TruckClassifications

            #region Pits
            context.Pits.AddOrUpdate(x => x.Name,
                new Pit() { ID = 1, Name = "Talons Cove Pit" },
                new Pit() { ID = 2, Name = "Grantsville Pit" },
                new Pit() { ID = 3, Name = "West Jordan Pit" },
                new Pit() { ID = 4, Name = "Glenwood Pit" },
                new Pit() { ID = 5, Name = "Traverse Mountain Pit" },
                new Pit() { ID = 6, Name = "Elberta Pit" },
                new Pit() { ID = 7, Name = "Mobile" },
                new Pit() { ID = 8, Name = "Mobile TC" },
                new Pit() { ID = 9, Name = "Talons Cove 2" },
                new Pit() { ID = 10, Name = "Cleanup West Jordan" },
                new Pit() { ID = 13, Name = "TC Scale" },
                new Pit() { ID = 14, Name = "WJ Scale" },
                new Pit() { ID = 15, Name = "GV Scale" },
                new Pit() { ID = 16, Name = "GW Scale" },
                new Pit() { ID = 17, Name = "Stansbury Island Pit" },
                new Pit() { ID = 18, Name = "Stabsury Island Scale" },
                new Pit() { ID = 19, Name = "Mobile Impactor" },
                new Pit() { ID = 20, Name = "Herriman Pit" },
                new Pit() { ID = 21, Name = "Combined Talons" },
                new Pit() { ID = 22, Name = "Quality Control" }
                );

            #endregion

            #region OverheadCodes

            context.OverheadCodes.AddOrUpdate(x => new { x.DepartmentID, x.Type },
                new OverheadCode()
                {
                    DepartmentID = (int)DepartmentName.Development,
                    Type = "Grease",
                    JobNumber = "JOB",
                    PhaseNumber = "GEN",
                    CategoryNumber = "GREASE"
                },
                new OverheadCode()
                {
                    DepartmentID = (int)DepartmentName.Development,
                    Type = "Daily",
                    JobNumber = "JOB",
                    PhaseNumber = "GEN",
                    CategoryNumber = "PAPER"
                },
                new OverheadCode()
                {
                    DepartmentID = (int)DepartmentName.Development,
                    Type = "Shop",
                    JobNumber = "JOB",
                    PhaseNumber = "GEN",
                    CategoryNumber = "PREP"
                },
                new OverheadCode()
                {
                    DepartmentID = (int)DepartmentName.Development,
                    Type = "Travel",
                    JobNumber = "JOB",
                    PhaseNumber = "GEN",
                    CategoryNumber = "TRAVEL"
                },
                new OverheadCode()
                {
                    DepartmentID = (int)DepartmentName.Residential,
                    Type = "Grease",
                    JobNumber = "LOTOH",
                    PhaseNumber = "GEN",
                    CategoryNumber = "8GEN"
                },
                new OverheadCode()
                {
                    DepartmentID = (int)DepartmentName.Residential,
                    Type = "Daily",
                    JobNumber = "LOTOH",
                    PhaseNumber = "GEN",
                    CategoryNumber = "8GEN"
                },
                new OverheadCode()
                {
                    DepartmentID = (int)DepartmentName.Residential,
                    Type = "Shop",
                    JobNumber = "LOTOH",
                    PhaseNumber = "GEN",
                    CategoryNumber = "8GEN"
                },
                new OverheadCode()
                {
                    DepartmentID = (int)DepartmentName.Residential,
                    Type = "Travel",
                    JobNumber = "LOTOH",
                    PhaseNumber = "GEN",
                    CategoryNumber = "8TT"
                },
                new OverheadCode()
                {
                    DepartmentID = (int)DepartmentName.Concrete,
                    Type = "Grease",
                    JobNumber = "FWMISC",
                    PhaseNumber = "FIX",
                    CategoryNumber = "CODE"
                },
                new OverheadCode()
                {
                    DepartmentID = (int)DepartmentName.Concrete,
                    Type = "Daily",
                    JobNumber = "FWMISC",
                    PhaseNumber = "FIX",
                    CategoryNumber = "CODE"
                },
                new OverheadCode()
                {
                    DepartmentID = (int)DepartmentName.Concrete,
                    Type = "Shop",
                    JobNumber = "FWMISC",
                    PhaseNumber = "FIX",
                    CategoryNumber = "CODE"
                },
                new OverheadCode()
                {
                    DepartmentID = (int)DepartmentName.Concrete,
                    Type = "Travel",
                    JobNumber = "FWMISC",
                    PhaseNumber = "FIX",
                    CategoryNumber = "CODE"
                },
                new OverheadCode()
                {
                    DepartmentID = (int)DepartmentName.Concrete2H,
                    Type = "Grease",
                    JobNumber = "FWMISC",
                    PhaseNumber = "FIX",
                    CategoryNumber = "CODE"
                },
                new OverheadCode()
                {
                    DepartmentID = (int)DepartmentName.Concrete2H,
                    Type = "Daily",
                    JobNumber = "FWMISC",
                    PhaseNumber = "FIX",
                    CategoryNumber = "CODE"
                },
                new OverheadCode()
                {
                    DepartmentID = (int)DepartmentName.Concrete2H,
                    Type = "Shop",
                    JobNumber = "FWMISC",
                    PhaseNumber = "FIX",
                    CategoryNumber = "CODE"
                },
                new OverheadCode()
                {
                    DepartmentID = (int)DepartmentName.Concrete2H,
                    Type = "Travel",
                    JobNumber = "FWMISC",
                    PhaseNumber = "FIX",
                    CategoryNumber = "CODE"
                },
                new OverheadCode()
                {
                    DepartmentID = (int)DepartmentName.ConcreteHB,
                    Type = "Grease",
                    JobNumber = "FWMISC",
                    PhaseNumber = "FIX",
                    CategoryNumber = "CODE"
                },
                new OverheadCode()
                {
                    DepartmentID = (int)DepartmentName.ConcreteHB,
                    Type = "Daily",
                    JobNumber = "FWMISC",
                    PhaseNumber = "FIX",
                    CategoryNumber = "CODE"
                },
                new OverheadCode()
                {
                    DepartmentID = (int)DepartmentName.ConcreteHB,
                    Type = "Shop",
                    JobNumber = "FWMISC",
                    PhaseNumber = "FIX",
                    CategoryNumber = "CODE"
                },
                new OverheadCode()
                {
                    DepartmentID = (int)DepartmentName.ConcreteHB,
                    Type = "Travel",
                    JobNumber = "FWMISC",
                    PhaseNumber = "FIX",
                    CategoryNumber = "CODE"
                }
                );

            #endregion

            #region NoteTypes

            context.NoteTypes.AddOrUpdate(x => x.ID, new NoteType()
            {
                ID = (int)NoteTypeName.ClockIn,
                Name = NoteTypeName.ClockIn.ToString(),
                Description = "Clock In",
                IsSystemGenerated = false
            },
            new NoteType()
            {
                ID = (int)NoteTypeName.ClockOut,
                Name = NoteTypeName.ClockOut.ToString(),
                Description = "Clock Out",
                IsSystemGenerated = false
            },
            new NoteType()
            {
                ID = (int)NoteTypeName.Injury,
                Name = NoteTypeName.Injury.ToString(),
                Description = "Injury",
                IsSystemGenerated = false
            },
            new NoteType()
            {
                ID = (int)NoteTypeName.InvalidClockOut,
                Name = NoteTypeName.InvalidClockOut.ToString(),
                Description = "Invalid Clock Out",
                IsSystemGenerated = true
            },
            new NoteType()
            {
                ID = (int)NoteTypeName.TooManyHours,
                Name = NoteTypeName.TooManyHours.ToString(),
                Description = "Too Many Hours",
                IsSystemGenerated = true
            },
            new NoteType()
            {
                ID = (int)NoteTypeName.Missing,
                Name = NoteTypeName.Missing.ToString(),
                Description = "No Timers",
                IsSystemGenerated = true
            },
            new NoteType()
            {
                ID = (int)NoteTypeName.Other,
                Name = NoteTypeName.Other.ToString(),
                Description = "User Note",
                IsSystemGenerated = false
            },
            new NoteType()
            {
                ID = (int)NoteTypeName.OverAllocated,
                Name = NoteTypeName.OverAllocated.ToString(),
                Description = "Over Allocated Time",
                IsSystemGenerated = true
            },
            new NoteType()
            {
                ID = (int)NoteTypeName.UnderAllocated,
                Name = NoteTypeName.UnderAllocated.ToString(),
                Description = "Under Allocated Time",
                IsSystemGenerated = true
            },
            new NoteType()
            {
                ID = (int)NoteTypeName.Occurrence,
                Name = NoteTypeName.Occurrence.ToString(),
                Description = "Occurrence",
                IsSystemGenerated = false
            },
            new NoteType()
            {
                ID = (int)NoteTypeName.AuthorizeNote,
                Name = NoteTypeName.AuthorizeNote.ToString(),
                Description = "Authorize Note",
                IsSystemGenerated = false
            });

            #endregion NoteTypes

            #region Users
#if DEBUG
            context.Employees.AddOrUpdate(x => x.Username,
                new Employee()
                {
                    ID = 1,
                    Username = "admin",
                    Password = PasswordHash.CreateHash("password"),
                    Name = "Admin",
                    Pin = "1234",
                    Status = EntityStatus.Active,
                    EmployeeNumber = "Admin",
                    EmployeeTypeID = 3,
                    Roles = new List<Role> { sysAdmin }
                });
#endif
            context.Employees.AddOrUpdate(x => x.Username,
                new Employee()
                {
                    Username = "system",
                    Password = PasswordHash.CreateHash("password"),
                    Name = "System",
                    Pin = "1234",
                    Status = EntityStatus.Inactive,
                    EmployeeNumber = "System",
                    EmployeeTypeID = 3
                });
            #endregion Users

            #region RoleAuthActivities

            foreach (int activityId in Enum.GetValues(typeof(AuthActivityID)))
            {
                if (!context.RoleAuthActivities.Any(x => x.RoleID == sysAdmin.ID && x.AuthActivityID == activityId))
                {
                    try
                    {
                        context.RoleAuthActivities.Add(new RoleAuthActivity()
                        {
                            RoleID = sysAdmin.ID,
                            AuthActivityID = activityId,
                            AllDepartments = true,
                            OwnDepartments = false,
                        });
                        context.SaveChanges();
                    }
                    catch (DbUpdateException ex)
                    {
                        var errorMessage = $"Error giving admin permission to {Enum.Parse(typeof(AuthActivityID), activityId.ToString())}. Please check if it was added in the seed method";
                        throw new DbUpdateException(errorMessage, ex);
                    }
                }
            }

            #endregion RoleAuthActivities

            try
            {
                // Your code...
                // Could also be before try if you know the exception occurs in SaveChanges

                context.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var eve in e.EntityValidationErrors)
                {
                    sb.AppendFormat("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        sb.AppendFormat("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw new DbEntityValidationException("Entity Validation Failed - errors follow:\n" + sb.ToString(), e);
            }
        }
    }
}
