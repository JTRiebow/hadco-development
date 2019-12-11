using Hadco.Data.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Hadco.Common.DataTransferObjects;

namespace Hadco.Data
{
    public class EmployeeTimerRepository : GenericRepository<EmployeeTimer>, IEmployeeTimerRepository
    {
        public bool CanSupervisorApproveTimers(int supervisorEmployeeID, int[] employeeTimerIDs)
        {
            using (var sc = new SqlConnection(Context.Database.Connection.ConnectionString))
            {
                return sc.Query<int>(@"
                                    select count(*)
                                    from EmployeeTimers et
                                    join EmployeeSupervisors es on et.EmployeeID = es.EmployeeID
                                    where es.SupervisorID = @supervisorEmployeeID
                                    and et.EmployeeTimerID in @employeeTimerIDs
                                    ", new { supervisorEmployeeID, employeeTimerIDs })
                                    .Single() == employeeTimerIDs.Count(); // if the counts are the same then they can approve all of them.
            }
        }

        public void SupervisorApproveTimers(int supervisorEmployeeID, int[] employeeTimerIDs)
        {
            ApproveTimers(supervisorEmployeeID, employeeTimerIDs, "Supervisor");
        }

        public void AccountingApproveTimers(int accountingEmployeeID, int[] employeeTimerIDs)
        {
            ApproveTimers(accountingEmployeeID, employeeTimerIDs, "Accounting");
        }
        private void ApproveTimers(int employeeID, int[] employeeTimerIDs, string approvalType)
        {
            using (var sc = new SqlConnection(Context.Database.Connection.ConnectionString))
            {
                sc.Execute($@"
                        update EmployeeTimers
                        set ApprovedBy{approvalType} = 1, 
                        ApprovedBy{approvalType}EmployeeID = @employeeID,
                        ApprovedBy{approvalType}Time = getdate()
                        where EmployeeTimerID in @employeeTimerIDs
                        ", new { employeeID, employeeTimerIDs });
            }
        }

        public ForemanEmployeeTimer GetForemanEmployeeTimer(int timesheetID, int employeeTimerID)
        {
            return GetForemanEmployeeTimers(timesheetID, employeeTimerID).EmployeeTimers.FirstOrDefault();
        }

        public ForemanTimesheet GetForemanEmployeeTimers(int timesheetID)
        {
            return GetForemanEmployeeTimers(timesheetID, employeeTimerID: null);
        }
        private ForemanTimesheet GetForemanEmployeeTimers(int timesheetID, int? employeeTimerID)
        {
            using (var sc = new SqlConnection(Context.Database.Connection.ConnectionString))
            {
                sc.Open();
                var mapped = sc.QueryMultiple(@"          
                           select distinct et.EmployeeTimerID, ejt.EmployeeJobTimerID ID, jt.JobTimerID, ejt.LaborMinutes,
                            c.JobNumber + '-' + c.PhaseNumber + '-' + c.CategoryNumber JobPhaseCategory,
                            c.JobID, c.PhaseID, c.CategoryID, c.JobNumber, c.PhaseNumber, c.CategoryNumber, 
                            c.UnitsOfMeasure, c.PlannedQuantity, jt.NewQuantity, jt.Diary, jt.InvoiceNumber,
                            dbo.GetOtherNewQuantity(jt.JobTimerID) OtherNewQuantity, dbo.GetPreviousQuantity(jt.JobTimerID) PreviousQuantity
                            from EmployeeTimers et
                            cross apply 
                            	(select jt.JobTimerID, jt.CategoryID, jt.Diary, jt.NewQuantity, jt.InvoiceNumber
                            		from JobTimers jt 
                            		where jt.TimesheetID = @timesheetID
                            	) jt
                            left join EmployeeJobTimers ejt on et.EmployeeTimerID = ejt.EmployeeTimerID and jt.JobTimerID = ejt.JobTimerID
                            join Categories c on jt.CategoryID = c.CategoryID
                            where et.TimesheetID = @timesheetID
                            and (@employeeTimerID is null or et.EmployeeTimerID = @employeeTimerID)
                            
                            select distinct et.EmployeeTimerID, jt.JobTimerID, ejt.EmployeeJobTimerID, ejet.EmployeeJobEquipmentTimerID ID, ejet.EquipmentMinutes, e.EquipmentID, e.EquipmentNumber
                            from EmployeeTimers et
                            cross apply 
                            	(select jt.JobTimerID
                            		from JobTimers jt 
                            		where jt.TimesheetID = @timesheetID
                            	) jt
                            cross apply 
                              	(select e.EquipmentID, e.EquipmentNumber
                            		from Equipment e 
                            		join EmployeeJobEquipmentTimers ejet on e.EquipmentID = ejet.EquipmentID
                            		join EmployeeJobTimers ejt on ejet.EmployeeJobTimerID = ejt.EmployeeJobTimerID
                            		where ejt.JobTimerID = jt.JobTimerID
                            	) e
                            left join EmployeeJobTimers ejt on et.EmployeeTimerID = ejt.EmployeeTimerID and jt.JobTimerID = ejt.JobTimerID
                            left join EmployeeJobEquipmentTimers ejet on ejt.EmployeeJobTimerID = ejet.EmployeeJobTimerID and e.EquipmentID = ejet.EquipmentID
                            where et.TimesheetID = @timesheetID
                            and (@employeeTimerID is null or et.EmployeeTimerID = @employeeTimerID)",
                                                new { timesheetID, employeeTimerID }).Map<ForemanEmployeeJobTimer, ForemanEmployeeJobEquipmentTimer, Tuple<int, int>>(
                                            ejt => new Tuple<int, int>(ejt.EmployeeTimerID, ejt.JobTimerID),
                                            ejet => new Tuple<int, int>(ejet.EmployeeTimerID, ejet.JobTimerID),
                                            (ejt, ejet) => { ejt.EmployeeJobEquipmentTimers = ejet.OrderBy(x => x.EquipmentNumber); }).ToList();

                var employeeTimers = sc.QueryMultiple(@"
 select distinct et.EmployeeTimerID ID, et.TimesheetID, et.Injured, et.Day, et.EmployeeID, et.Submitted, et.DepartmentID, 
 		e.Name EmployeeName, et.EmployeeTimecardID, et.SubDepartmentID, et.ShopMinutes, et.TravelMinutes, et.GreaseMinutes, et.DailyMinutes,
         min(ete.ClockIn) StartTime, max(ete.ClockOut) EndTime, 
 		round(sum(dbo.GetTimeSpanHours(ete.ClockIn, ete.ClockOut))*60, 0) TotalMinutes, 
 		case when n.NoteID IS NULL THEN 0 else 1 end HasNote,
        da.ApprovedBySupervisor,
        da.ApprovedByBilling,
        da.ApprovedByAccounting
 from EmployeeTimers et
     left join EmployeeTimerEntries ete on et.EmployeeTimerID = ete.EmployeeTimerID
     join Employees e on et.EmployeeID = e.EmployeeID
	 outer apply (SELECT TOP 1 * FROM Notes n 
	   WHERE n.EmployeeID = et.EmployeeID AND n.Day = et.Day AND n.DepartmentID = et.DepartmentID and n.Resolved = 0) n
	 outer apply (SELECT TOP 1 * FROM DailyApprovals da 
	   WHERE da.EmployeeID = et.EmployeeID AND da.Day = et.Day AND da.DepartmentID = et.DepartmentID) da
 where et.TimesheetID = @timesheetID
 and (@employeeTimerID is null or et.EmployeeTimerID = @employeeTimerID)
 group by et.EmployeeTimerID, et.TimesheetID, et.Injured, et.Day, et.EmployeeID, et.Submitted, et.DepartmentID, 
 		e.Name, et.EmployeeTimecardID, et.SubDepartmentID, et.ShopMinutes, et.TravelMinutes, et.GreaseMinutes, et.DailyMinutes,
n.NoteID, da.ApprovedBySupervisor, da.ApprovedByBilling, da.ApprovedByAccounting;

 select eto.EmployeeTimerID, eto.OccurrenceID, o.Name
 from EmployeTimerOccurrences eto
     join Occurrences o on eto.OccurrenceID = o.OccurrenceID
     join EmployeeTimers et on eto.EmployeeTimerID = et.EmployeeTimerID
 where et.TimesheetID = @timesheetID;
                        ", new { timesheetID, employeeTimerID }).Map<ForemanEmployeeTimer, EmployeeTimerOccurrence, int>(
                    t => t.ID,
                    o => o.EmployeeTimerID,
                    (t, o) => { t.Occurrences = o; }).ToList();

                foreach (var employeeTimer in employeeTimers)
                {
                    employeeTimer.EmployeeJobTimers =
                        mapped.Where(x => x.EmployeeTimerID == employeeTimer.ID).OrderBy(x=>x.JobTimerID);
                }

                var timesheet = new ForemanTimesheet() {TimesheetID = timesheetID, EmployeeTimers = employeeTimers};

                return timesheet;
            }
        }

        public EmployeeTimer ReplaceEmployeeJobTimerRange(EmployeeTimer entity)
        {
            var existingModifier = Find(entity.ID, x => x.EmployeeJobTimers);
            if (existingModifier == null)
            {
                return null;
            }
            Context.Entry(existingModifier).CurrentValues.SetValues(entity);
            var newEmployeeJobTimers = entity.EmployeeJobTimers.ToList();
            var existingEmployeeeJobTimers = existingModifier.EmployeeJobTimers.ToList();
            foreach (var existingEmployeeJobTimer in existingEmployeeeJobTimers)
            {
                if (newEmployeeJobTimers.All(x => x.ID != existingEmployeeJobTimer.ID))
                {
                    Context.EmployeeJobTimers.Remove(existingEmployeeJobTimer);
                }
            }

            foreach (var newEmployeeJobTimer in newEmployeeJobTimers)
            {
                var oldEmployeeJobTimer =
                    existingEmployeeeJobTimers.SingleOrDefault(x => x.ID == newEmployeeJobTimer.ID);
                if (oldEmployeeJobTimer == null)
                {
                    newEmployeeJobTimer.EmployeeTimerID = entity.ID;
                    if (newEmployeeJobTimer.EmployeeJobEquipmentTimers.Any())
                    {
                        newEmployeeJobTimer.EmployeeJobEquipmentTimers?.ToList()
                            .ForEach(x => x.EmployeeJobTimer = newEmployeeJobTimer);
                        Context.EmployeeJobEquipmentTimers.AddRange(newEmployeeJobTimer.EmployeeJobEquipmentTimers);
                    }
                    else
                    {
                        var attachedEmployeeJobTimer = Context.EmployeeJobTimers.Attach(newEmployeeJobTimer);
                        Context.EmployeeJobTimers.Add(attachedEmployeeJobTimer);
                    }
                }
                else
                {
                    var newEmployeeJobEquipmentTimers = newEmployeeJobTimer.EmployeeJobEquipmentTimers;
                    newEmployeeJobTimer.EmployeeJobEquipmentTimers = null;

                    Context.Entry(oldEmployeeJobTimer).CurrentValues.SetValues(newEmployeeJobTimer);

                    var existingEmployeeJobEquipmentTimers = Context.EmployeeJobEquipmentTimers.Where(x => x.EmployeeJobTimerID == newEmployeeJobTimer.ID).ToList();
                    foreach (var existingEmployeeJobEquipmentTimer in existingEmployeeJobEquipmentTimers)
                    {
                        if (newEmployeeJobEquipmentTimers.All(x => x.ID != existingEmployeeJobEquipmentTimer.ID))
                        {
                            Context.EmployeeJobEquipmentTimers.Remove(existingEmployeeJobEquipmentTimer);
                        }
                    }

                    foreach (var newEmployeeJobEquipmentTimer in newEmployeeJobEquipmentTimers)
                    {
                        var oldEmployeeJobEquipmentTimer =
                            existingEmployeeJobEquipmentTimers.FirstOrDefault(
                                x => x.ID == newEmployeeJobEquipmentTimer.ID);
                        if (oldEmployeeJobEquipmentTimer == null)
                        {
                            if (newEmployeeJobTimer.ID != 0)
                            {
                                newEmployeeJobEquipmentTimer.EmployeeJobTimerID = newEmployeeJobTimer.ID;
                            }
                            var attachedEmployeeJobEquipmentTimer = Context.EmployeeJobEquipmentTimers.Attach(newEmployeeJobEquipmentTimer);
                            Context.EmployeeJobEquipmentTimers.Add(attachedEmployeeJobEquipmentTimer);
                        }
                        else
                        {
                            Context.Entry(oldEmployeeJobEquipmentTimer).CurrentValues.SetValues(newEmployeeJobEquipmentTimer);
                        }
                    }
                }
            }

            Save();
            return existingModifier;
        }

        #region EmployeeOccurrences
        private IQueryable<Occurrence> _occurrenceEntities = null;

        public IQueryable<Occurrence> GetOccurrencesForEmployeeTimer(int employeeTimerID)
        {
            return _occurrenceEntities = _occurrenceEntities ?? Context.EmployeeTimers.Where(u => u.ID == employeeTimerID).SelectMany(u => u.Occurrences);
        }

        public Occurrence AddOccurrence(int employeeTimerID, Occurrence entity)
        {
            var existingEmployeeTimer = Context.EmployeeTimers.Include("Occurrences").FirstOrDefault(u => u.ID == employeeTimerID);
            if (Context.Entry(entity).State == System.Data.Entity.EntityState.Detached)
                Context.Occurrences.Attach(entity);

            existingEmployeeTimer?.Occurrences.Add(entity);
            Save();
            return entity;
        }

        public IEnumerable<Occurrence> ReplaceOccurrences(int employeeTimerID, IEnumerable<int> occurrences)
        {
            using (var sc = new SqlConnection(Context.Database.Connection.ConnectionString))
            {
                sc.Open();
                sc.Query(@"delete from EmployeTimerOccurrences 
                                        where EmployeeTimerID = @employeeTimerID",
                    new { employeeTimerID });
                
                var query = "";
                if (occurrences.Count() > 0) {
                    query = occurrences.Aggregate(@"insert into EmployeTimerOccurrences values", (current, occurrence) => current + $@" ({employeeTimerID}, {occurrence}),").TrimEnd(',');
                    query += ';';
                }
                query +=
                    $@" select o.OccurrenceID ID, o.Name, o.Code 
                        from Occurrences o 
                        where o.OccurrenceID in 
                            (select OccurrenceID 
                             from EmployeTimerOccurrences eto
                            where eto.EmployeeTimerID = {employeeTimerID})";
                return sc.Query<Occurrence>(query);
            }

        }

        public void RemoveOccurrence(int employeeTimerID, int occurrenceID)
        {
            var existingEmployee = Context.EmployeeTimers.Include("Occurrences").FirstOrDefault(u => u.ID == employeeTimerID);
            var occurrenceItem = Context.Occurrences.FirstOrDefault(d => d.ID == occurrenceID);
            existingEmployee?.Occurrences.Remove(occurrenceItem);
            Save();
        }
        #endregion

        public override EmployeeTimer Update(EmployeeTimer entity)
        {

            Context.Entry(entity).State = EntityState.Modified;
            Context.Entry(entity.AuthorizeNote).State = entity.AuthorizeNoteID == 0 ?
                                                        EntityState.Added :
                                                        EntityState.Modified;  
            Save();
            return entity;
        }

    }

    public interface IEmployeeTimerRepository : IGenericRepository<EmployeeTimer>
    {
        bool CanSupervisorApproveTimers(int supervisorEmployeeID, int[] timecardIDs);
        void SupervisorApproveTimers(int supervisorEmployeeID, int[] timecardIDs);
        void AccountingApproveTimers(int accountingEmployeeID, int[] timecardIDs);
        EmployeeTimer ReplaceEmployeeJobTimerRange(EmployeeTimer entity);
        ForemanEmployeeTimer GetForemanEmployeeTimer(int timesheetID, int employeeTimerID);
        ForemanTimesheet GetForemanEmployeeTimers(int timesheetID);
        IQueryable<Occurrence> GetOccurrencesForEmployeeTimer(int employeeTimerID);
        Occurrence AddOccurrence(int employeeTimerID, Occurrence entity);
        IEnumerable<Occurrence> ReplaceOccurrences(int employeeTimerID, IEnumerable<int> entity);
        void RemoveOccurrence(int employeeTimerID, int occurrenceID);
    }
}
