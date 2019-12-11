--Mechanics- Equipment Timers (All Employees in Mechanics)
--EmployeeNumber(P. SAMA), Day, TotalHours, EquipmentNumber, EquipmentServiceType
 select e.EmployeeNumber, CONVERT(nvarchar(10),ts.Day, 101) Day, ltrim(str(sum(dbo.GetTimeSpanHours(ete.StartTime, ete.StopTime)), 5,2)) TotalHours, eq.EquipmentNumber, est.Code EquipmentServiceType
 from equipment eq
 join EquipmentTimers et on eq.EquipmentID = et.EquipmentID
 join EquipmentTimerEntries ete on et.EquipmentTimerID = ete.EquipmentTimerID
 join EquipmentServiceTypes est on ete.EquipmentServiceTypeID = est.EquipmentServiceTypeID
 join timesheets ts on et.timesheetid = ts.timesheetid
 join Employees e on ts.EmployeeID = e.EmployeeID
 where ts.Day between @startDate and @endDate
 and (ts.departmentid in @departmentID)
 and ete.StopTime is not null
 and datediff(mi, ete.StartTime, ete.StopTime) > 0
 group by e.EmployeeNumber, ts.Day, eq.equipmentnumber, est.Code