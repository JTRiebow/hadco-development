using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadco.Integration.ComputerEase
{
    public class Table
    {
        public Table(string tableName, string odbcSelectText, string migrationSql, string partialOdbcSelectText = null, string partialMigrationSql = null)
        {
            TableName = tableName;
            OdbcSelectText = odbcSelectText;
            MigrationSql = migrationSql;
            PartialMigrationSql = partialMigrationSql ?? migrationSql;
            PartialOdbcSelectText = partialOdbcSelectText ?? odbcSelectText;
        }

        public string TableName { get; set; }
        public string OdbcSelectText { get; set; }
        public string MigrationSql { get; set; }
        public string PartialOdbcSelectText { get; set; }
        public string PartialMigrationSql { get; set; }


        public static List<Table> GetTables()
        {
            return new List<Table>() {
                new Table("Customers", @"select c.cusnum, c.name, c.address1, c.address2, c.addresscity, c.addressstate, c.addresszip, c.phonenum, c.notes1, c.notes2, c.contact, c.email, c.memo
                                        from customer c"
                                       ,@"    Merge dbo.Customers as t
                                            Using [raw].Customers as s
                                            on (t.CustomerNumber = s.cusnum)
                                            WHEN NOT MATCHED BY TARGET
                                                THEN INSERT(CustomerNumber, Name) VALUES(cusnum, name)
                                            WHEN MATCHED
                                                THEN UPDATE SET t.Name = s.Name
                                            --WHEN NOT MATCHED BY SOURCE
                                            --  THEN DELETE
                                            OUTPUT $action, Inserted.*, Deleted.*;"),
                new Table("Equipment", @"SELECT e.equipnum, e.name, e.model, e.license, e.location, e.fleetcode, e.serialnum1, e.licenseexpdate, e.purchasedate,
                                               e.mileage, e.hoursofoperation, e.estimatedcost, e.lastserviced, e.unpostedhours, e.purchasemiles, e.purchasehours,
                                               e.memo, e.estimatedcostown, e.unpostedhoursown, e.status, e.serial, e.user_3, e.user_1, e.user_2
                                        FROM equipment e",
                                        @"    Merge dbo.Equipment as t
                                            Using [raw].Equipment as s
                                            on (t.EquipmentNumber = s.equipnum)
                                            WHEN NOT MATCHED BY TARGET
                                                THEN INSERT(EquipmentNumber, Name, Model, License, Fleetcode, SerialNumber, Mileage, HoursOfOperation, Memo, [Status], [Type])
                                                VALUES(s.equipnum, s.name, s.model, s.license, s.fleetcode, s.serialnum1, s.mileage, s.hoursofoperation, s.memo, s.[status], s.user_3)
                                            WHEN MATCHED
                                                THEN UPDATE SET Name = s.name, Model = s.model, License = s.license, Fleetcode = s.fleetcode, SerialNumber = s.serialnum1,
                                                                Mileage = s.mileage, HoursOfOperation = s.hoursofoperation, Memo = s.memo, t.[Status] = s.[status], t.[Type] = s.user_3
	                                        --WHEN NOT MATCHED BY SOURCE
		                                        --THEN DELETE
                                            OUTPUT $action, Inserted.*, Deleted.*;",
                                         @"SELECT e.equipnum, e.name, e.model, e.license, e.location, e.fleetcode, e.serialnum1, e.licenseexpdate, e.purchasedate,
                                               e.mileage, e.hoursofoperation, e.estimatedcost, e.lastserviced, e.unpostedhours, e.purchasemiles, e.purchasehours,
                                               e.memo, e.estimatedcostown, e.unpostedhoursown, e.status, e.serial, e.user_3, e.user_1, e.user_2
                                        FROM equipment e
                                        where e.status =1"),
                new Table("Employees", @"SELECT e.empnum, e.name, e.address1, e.address2, e.address3, e.phonenum, e.department, e.location, e.class, e.addresscity, 
                                        e.addressstate, e.addresszip, e.lastname, e.firstname, e.middlename, e.birthdate, e.sex, e.married, e.type, e.ratetype, e.payrate, 
                                        e.overtimemult, e.memo, e.executivepr, e.weeklyhours, e.status, e.ssnum
                                        FROM premployee e",
                                        @"Merge dbo.Employees as t
                                            Using (select * from [raw].Employees) as s
                                            on (t.EmployeeNumber = s.empnum)
                                            WHEN NOT MATCHED BY TARGET
                                                THEN INSERT(EmployeeNumber, Name, Phone, [Status], Username, [Password], Pin, EmployeeTypeID, Location) VALUES(empnum, name, phonenum, [status], lower(empnum), '1000:YUPGPUDgjm/KNZofuvgsjmWDzWiRGXwL:Oc9s5V/fGlj6mK2k7m3/+2+nbSIHpF8r', right(ssnum, 4), type, location)
                                            WHEN MATCHED
                                                THEN UPDATE SET t.Name = s.Name, t.Phone = s.phonenum, t.[Status] = s.[status], t.EmployeeTypeID = s.type, t.Location = s.location
                                            --WHEN NOT MATCHED BY SOURCE
                                                --THEN DELETE
                                            OUTPUT $action, Inserted.*, Deleted.*;

                                            insert into EmployeeDepartments
                                            select e1.EmployeeID, d.DepartmentID
                                            from raw.Employees e
                                            join dbo.Employees e1 on e.empnum = e1.EmployeeNumber
                                            cross apply (select d.DepartmentID from dbo.Departments d where case when e.location in ('FINISH', 'FOUNDATION', 'CONCRETE') then 'Concrete' 
																	                                             when e.location = 'MAINLINE' then 'Development' 
																	                                             when e.location = 'RESIDENT' then 'Residential'																	 
																	                                             when e.location in ('TRUCK-DEV', 'TRUCK-RES') then 'Trucking' 
                                                                                                                 when e.location = 'TRANSPORT' then 'Transport'
																	                                             when e.location = 'MECHANICS' then 'Mechanic'
																	                                             when e.location = 'OFFICE' then 'Front Office'
																                                             end= d.Name) d
                                            where e.status = 1
                                            and not exists (select * from EmployeeDepartments ed where ed.EmployeeID = e1.EmployeeID);

                                            insert into EmployeeRoles
                                            select distinct e.EmployeeID, 2 RoleID
                                            from dbo.Employees e
                                            join dbo.EmployeeDepartments er on e.EmployeeID = er.EmployeeID
                                            join dbo.Departments d on er.DepartmentID = d.DepartmentID
                                            where e.status = 1
                                            and d.Name in ('Trucking', 'Front Office', 'Mechanic', 'TM Crushing')
                                            and not exists (select * from EmployeeRoles er where er.EmployeeID = e.EmployeeID and er.RoleId = 2)"
                                        ,
                                        @"SELECT e.empnum, e.name, e.address1, e.address2, e.address3, e.phonenum, e.department, e.location, e.class, e.addresscity, 
                                        e.addressstate, e.addresszip, e.lastname, e.firstname, e.middlename, e.birthdate, e.sex, e.married, e.type, e.ratetype, e.payrate, 
                                        e.overtimemult, e.memo, e.executivepr, e.weeklyhours, e.status, e.ssnum
                                        FROM premployee e
                                        where e.Status = 1"),
                new Table("Jobs", @"SELECT j.jobnum, j.name, j.address1, j.addresscity, j.addressstate, j.addresszip, j.cusnum, j.class, j.dateopen, j.status, j.memo, j.user_1, j.user_2 
                                    FROM jcjob j
                                    WHERE j.status = 1 or j.status = 2",

                                  @"Merge dbo.Jobs as t
                                        Using (select j.JobNum, case when rtrim(j.name) = '' then j.JobNum else j.name end name, j.address1, j.addresscity, j.addressstate, j.addresszip, c.CustomerID, j.cusnum, j.class, j.dateopen, j.[status],
				                                        j.memo, j.user_2, j.user_1, ct.CustomerTypeID
                                                from [raw].Jobs j
                                                left join dbo.Customers c on j.cusnum = c.CustomerNumber
                                                outer apply(select ct.CustomerTypeID from dbo.CustomerTypes ct where case when j.class like 'P%' then 'Development' 
                                                                                        when j.class like 'L%' or j.class like 'F%' or j.class like 'J%' then 'Residential'
                                                                                        when j.class like 'S%' and j.JobNum = 'TKMISC' then 'Outside'
                                                                                        when j.class like 'S%' and j.JobNum = 'METRO' then 'Metro'
                                                                                    end = ct.Name) ct
                                                where j.jobnum != '') as s
                                        on (t.JobNumber = s.jobnum)
                                        WHEN NOT MATCHED BY TARGET
                                            THEN INSERT(JobNumber, Name, Address1, City, [State], Zip, CustomerID, CustomerNumber, Class, DateOpen, [Status], Memo, DateFiled, PreliminaryFilingNumber, CustomerTypeID, ModifiedDate)
                                            VALUES(s.JobNum, s.name, s.address1, s.addresscity, s.addressstate, s.addresszip, s.CustomerID, s.cusnum, s.class, s.dateopen, s.[status],
				                                        s.memo, s.user_2, s.user_1, s.CustomerTypeID, @getdate)
                                        WHEN MATCHED
                                            THEN UPDATE SET Name = s.name, Address1 = s.Address1, City = s.addresscity, [State] = s.addressstate, Zip = s.addresszip,
                                                            CustomerID = s.customerid, CustomerNumber = s.cusnum, Class = s.class, DateOpen = s.dateopen, [Status] = s.[status], 
					                                        Memo = s.memo, DateFiled = s.user_2, PreliminaryFilingNumber = s.user_1, CustomerTypeID = s.CustomerTypeID, ModifiedDate = @getdate  
                                        WHEN NOT MATCHED BY SOURCE
                                            THEN UPDATE SET Status = 2, ModifiedDate = @getdate                         
                                        OUTPUT $action, Inserted.*, Deleted.*;",

                                    @"SELECT j.jobnum, j.name, j.address1, j.addresscity, j.addressstate, j.addresszip, j.cusnum, j.class, j.dateopen, j.status, j.memo, j.user_1, j.user_2 
                                    FROM jcjob j
                                    WHERE j.status = 1
                                    AND (j.dateopen>"+ $@"{{d'{DateTime.Now.Date.AddDays(-1):yyyy-MM-dd}'}}" +")",

                                  @"Merge dbo.Jobs as t
                                        Using (select j.JobNum, case when rtrim(j.name) = '' then j.JobNum else j.name end name, j.address1, j.addresscity, j.addressstate, j.addresszip, c.CustomerID, j.cusnum, j.class, j.dateopen, j.[status],
				                                        j.memo, j.user_2, j.user_1, ct.CustomerTypeID
                                                from [raw].Jobs j
                                                left join dbo.Customers c on j.cusnum = c.CustomerNumber
                                                outer apply(select ct.CustomerTypeID from dbo.CustomerTypes ct where case when j.class like 'P%' then 'Development' 
                                                                                        when j.class like 'L%' or j.class like 'F%' or j.class like 'J%' then 'Residential'
                                                                                        when j.class like 'S%' and j.JobNum = 'TKMISC' then 'Outside'
                                                                                        when j.class like 'S%' and j.JobNum = 'METRO' then 'Metro'
                                                                                    end = ct.Name) ct
                                                where j.jobnum != '') as s
                                        on (t.JobNumber = s.jobnum)
                                        WHEN NOT MATCHED BY TARGET
                                            THEN INSERT(JobNumber, Name, Address1, City, [State], Zip, CustomerID, CustomerNumber, Class, DateOpen, [Status], Memo, DateFiled, PreliminaryFilingNumber, CustomerTypeID, ModifiedDate)
                                            VALUES(s.JobNum, s.name, s.address1, s.addresscity, s.addressstate, s.addresszip, s.CustomerID, s.cusnum, s.class, s.dateopen, s.[status],
				                                        s.memo, s.user_2, s.user_1, s.CustomerTypeID, @getdate)
                                        WHEN MATCHED
                                            THEN UPDATE SET Name = s.name, Address1 = s.Address1, City = s.addresscity, [State] = s.addressstate, Zip = s.addresszip,
                                                            CustomerID = s.customerid, CustomerNumber = s.cusnum, Class = s.class, DateOpen = s.dateopen, [Status] = s.[status], 
					                                        Memo = s.memo, DateFiled = s.user_2, PreliminaryFilingNumber = s.user_1, CustomerTypeID = s.CustomerTypeID, ModifiedDate = @getdate                             
                                        OUTPUT $action, Inserted.*, Deleted.*;"
                                    ),
                new Table("Phases", @"SELECT jp.sequence, jp.jobnum, jp.phasenum, jp.name, jp.retisfromjob
                                        FROM jcjob jcjob, jcphase jp
                                        WHERE jcjob.jobnum = jp.jobnum
                                        AND jcjob.status = 1",
                                    @"	Merge dbo.Phases as t
                                    Using (select p.phasenum, j.JobID, j.JobNumber, case when rtrim(p.name) = '' then p.PhaseNum else p.name end name
                                            from [raw].Phases p
                                            join dbo.Jobs j on p.jobnum = j.JobNumber
                                            where p.phasenum != '') as s
                                    on (t.PhaseNumber = s.phasenum and t.JobNumber = s.JobNumber)
                                    WHEN NOT MATCHED BY TARGET
                                        THEN INSERT(PhaseNumber, JobID, JobNumber, Name)
                                        VALUES(s.Phasenum, s.jobid, s.jobnumber, s.name)
                                    WHEN MATCHED
                                        THEN UPDATE SET Name = s.Name
                                    WHEN NOT MATCHED BY SOURCE
                                        THEN UPDATE SET Status = 3    
                                    OUTPUT $action, Inserted.*, Deleted.*;",
                                    @"SELECT jp.sequence, jp.jobnum, jp.phasenum, jp.name, jp.retisfromjob
                                        FROM jcjob jcjob, jcphase jp
                                        WHERE jcjob.jobnum = jp.jobnum
                                        AND jcjob.status = 1
                                        AND (jcjob.dateopen>"+ $@"{{d'{DateTime.Now.Date.AddDays(-1):yyyy-MM-dd}'}}" +")",

                                     @"	Merge dbo.Phases as t
                                    Using (select p.phasenum, j.JobID, j.JobNumber, case when rtrim(p.name) = '' then p.PhaseNum else p.name end name
                                            from [raw].Phases p
                                            join dbo.Jobs j on p.jobnum = j.JobNumber
                                            where p.phasenum != '') as s
                                    on (t.PhaseNumber = s.phasenum and t.JobNumber = s.JobNumber)
                                    WHEN NOT MATCHED BY TARGET
                                        THEN INSERT(PhaseNumber, JobID, JobNumber, Name)
                                        VALUES(s.Phasenum, s.jobid, s.jobnumber, s.name)
                                    WHEN MATCHED
                                        THEN UPDATE SET Name = s.Name
                                    OUTPUT $action, Inserted.*, Deleted.*;"
                                    ),
                new Table("Categories", @"SELECT  jc.sequence, jc.jobnum, jc.phasenum, jc.catnum, jc.name, jc.unitofmeasure, jc.unitsbudgeted
                                        FROM jcjob jcjob, jccat jc
                                        WHERE jcjob.jobnum = jc.jobnum
                                        AND jcjob.status = 1
                                        AND jc.catnum <> '' 
                                        {0}",

                                        @"	Merge dbo.Categories as t
                                            Using (select p.catnum, p.jobnum, j.Jobid, p.phasenum, j.PhaseID, 
                                                    case when rtrim(p.name) = '' then p.catnum else p.name end name,
                                                     p.unitofmeasure UnitsOfMeasure, p.unitsbudgeted PlannedQuantity
                                                    from [raw].Categories p
                                                    join dbo.Phases j on p.jobnum = j.JobNumber and p.phasenum = j.phasenumber
                                                    where p.catnum != '') as s
                                            on (t.CategoryNumber = s.catnum and t.phasenumber = s.phasenum and t.JobNumber = s.jobnum)
                                            WHEN NOT MATCHED BY TARGET
                                                THEN INSERT(CategoryNumber, JobNumber, PhaseNumber, PhaseID, Name, JobID, UnitsOfMeasure, PlannedQuantity)
                                                VALUES(s.catnum, s.jobnum, s.Phasenum, s.phaseid, s.name, s.JobID, s.UnitsOfMeasure, s.PlannedQuantity)
                                            WHEN NOT MATCHED BY SOURCE
                                                THEN UPDATE SET Status = 3
                                            WHEN MATCHED
                                                THEN UPDATE SET Name = s.Name, UnitsOfMeasure = s.UnitsOfMeasure, PlannedQuantity = s.PlannedQuantity
                                            OUTPUT $action, Inserted.*, Deleted.*;",

                                        @"SELECT  jc.sequence, jc.jobnum, jc.phasenum, jc.catnum, jc.name, jc.unitofmeasure, jc.unitsbudgeted
                                        FROM jcjob jcjob, jccat jc
                                        WHERE jcjob.jobnum = jc.jobnum
                                        AND jcjob.status = 1
                                        AND jc.catnum <> ''
                                        AND (jcjob.dateopen>"+ $@"{{d'{DateTime.Now.Date.AddDays(-1):yyyy-MM-dd}'}}" +")",

                                         @"	Merge dbo.Categories as t
                                            Using (select p.catnum, p.jobnum, j.Jobid, p.phasenum, j.PhaseID, 
                                                    case when rtrim(p.name) = '' then p.catnum else p.name end name,
                                                     p.unitofmeasure UnitsOfMeasure, p.unitsbudgeted PlannedQuantity
                                                    from [raw].Categories p
                                                    join dbo.Phases j on p.jobnum = j.JobNumber and p.phasenum = j.phasenumber
                                                    where p.catnum != '') as s
                                            on (t.CategoryNumber = s.catnum and t.phasenumber = s.phasenum and t.JobNumber = s.jobnum)
                                            WHEN NOT MATCHED BY TARGET
                                                THEN INSERT(CategoryNumber, JobNumber, PhaseNumber, PhaseID, Name, JobID, UnitsOfMeasure, PlannedQuantity)
                                                VALUES(s.catnum, s.jobnum, s.Phasenum, s.phaseid, s.name, s.JobID, s.UnitsOfMeasure, s.PlannedQuantity)
                                            WHEN MATCHED
                                                THEN UPDATE SET Name = s.Name, UnitsOfMeasure = s.UnitsOfMeasure, PlannedQuantity = s.PlannedQuantity
                                            OUTPUT $action, Inserted.*, Deleted.*;"
                                        )                
            };
        }
    }
}
