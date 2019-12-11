namespace Hadco.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class weeklyTimesheetSP : DbMigration
    {
        public override void Up()
        {
            Sql(@"create procedure dbo.GetWeeklyTimers 
	@week DATE,
	@supervisorID int = null,
	@departmentID int = null
as

	DECLARE @startOfWeek date, @endOfWeek date;

	select @startOfWeek = DATEADD(dd, -(DATEPART(dw, @week)-1), @week) 
	select @endOfWeek = DATEADD(dd, 7-(DATEPART(dw, @week)), @week) 
	;
	with
	OccurrencesByDay as
	(
		select p.DepartmentID, p.EmployeeID, p.StartOfWeek, isnull([Day0], '') [Day0], isnull([Day1], '') [Day1], isnull([Day2], '') [Day2], isnull([Day3], '') [Day3], isnull([Day4], '') [Day4], isnull([Day5], '') [Day5], isnull([Day6], '') [Day6]
		from 
		(
			select et.DepartmentID, et.EmployeeID,  @startOfWeek StartOfWeek, 'Day' + convert(varchar(1),DATEPART(dw, et.[Day]) -1) [DayOfWeek], t.OccurrenceCode
			from EmployeeTimers et		
			cross apply
			(
				SELECT left(o.Code, 1)
				FROM EmployeTimerOccurrences eto1
				join Occurrences o on eto1.OccurrenceID = o.OccurrenceID
				where et.EmployeeTimerID = eto1.EmployeeTimerID
				FOR XML PATH('')
			) as t (OccurrenceCode)
			where et.[Day] between @startOfWeek and @endOfWeek
		) as s
		pivot
		(
			max(OccurrenceCode)
			for [DayOfWeek] in ([Day0], [Day1], [Day2], [Day3], [Day4], [Day5], [Day6])
		) as P
	)
	,TimerEntries as
	(
		select p.DepartmentID, p.EmployeeID, p.StartOfWeek, isnull([Day0], '') [Day0], isnull([Day1], '') [Day1], isnull([Day2], '') [Day2], isnull([Day3], '') [Day3], isnull([Day4], '') [Day4], isnull([Day5], '') [Day5], isnull([Day6], '') [Day6]
		from
			(
			select et.DepartmentID, et.EmployeeID, @startOfWeek StartOfWeek, 'Day' + convert(varchar(1),DATEPART(dw, et.[Day]) -1) [DayOfWeek], ltrim(str(convert(decimal(9,2), datediff(mi, ete.ClockIn, ete.ClockOut))/convert(decimal(9,2),60.00), 5,2)) ClockedInHours
			from EmployeeTimers et
			join EmployeeTimerEntries ete on ete.EmployeeTimerID = et.EmployeeTimerID
			--group by et.DepartmentID, et.EmployeeID, et.[Day]
			where et.Day between @startOfWeek and @endOfWeek	
			) t
		pivot
		(
			max(ClockedInHours)
			for [DayOfWeek] in ([Day0], [Day1], [Day2], [Day3], [Day4], [Day5], [Day6])

		) as P
	)
	select te.DepartmentID, te.EmployeeID, e.Name, d.Name Department, left(t.Supervisor, len(t.supervisor)-1) Supervisor, te.[StartOfWeek], concat(te.Day0, obd.Day0) Day0, concat(te.Day1, obd.Day1) Day1, concat(te.Day2, obd.Day2) Day2, concat(te.Day3, obd.Day3) Day3, concat(te.Day4, obd.Day4) Day4, concat(te.Day5, obd.Day5) Day5, concat(te.Day6, obd.Day6) Day6
	from TimerEntries te 
	join OccurrencesByDay obd on te.EmployeeID = obd.EmployeeID and te.DepartmentID = obd.DepartmentID and te.StartOfWeek = obd.StartOfWeek
	join Employees e on te.EmployeeID = e.EmployeeID
	join Departments d on te.DepartmentID = d.DepartmentID
	cross apply
			(
				SELECT s.Name + ', '
				FROM EmployeeSupervisors es
				join Employees s on es.SupervisorID = s.EmployeeID
				where es.EmployeeID = e.EmployeeID
				FOR XML PATH('')
			) as t (Supervisor)
	where (@departmentID is null or te.DepartmentID = @departmentID)
	and (@supervisorID is null or exists (select * from EmployeeSupervisors es where e.EmployeeID = es.EmployeeID and es.SupervisorID = @supervisorID)) 

GO");
        }
        
        public override void Down()
        {
            Sql(@"drop procedure dbo.GetWeeklyTimers");
        }
    }
}
