using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Hadco.Common;
using Hadco.Common.DataTransferObjects;
using Hadco.Common.Enums;
using Hadco.Data.CsvObjects;
using Hadco.Data.Entities;

namespace Hadco.Data
{
    public class CsvRepository : ICsvRepository
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["Hadco.Data.HadcoContext"].ConnectionString;

        /// <summary>
        /// Some of these queries may not work on all versions of Sql Server, but they should all work on Azure Sql
        /// </summary>
        /// <param name="type"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="departmentID"></param>
        /// <returns></returns>
        public IEnumerable<CsvDto> GetCsv(CsvType type, DateTime startDate, DateTime endDate, DepartmentName[] departmentID)
        {
            var parameters = new { startDate, endDate, departmentID };
            switch (type)
            {
                case CsvType.LoadTimers:
                    return RunQuery<LoadTimerCsvDto>(Resources.GetLoadTimersCsv, parameters);
                case CsvType.Occurrences:
                    return RunQuery<OccurrencesCsvDto>(Resources.GetOccurrencesCsv, parameters);
                case CsvType.JobTimerConcrete:
                    return RunQuery<JobTimerConcreteCsvDto>(Resources.GetJobTimerCsv, parameters);
                case CsvType.JobTimerCdr:
                    return RunQuery<JobTimerCsvDto>(Resources.GetJobTimerCsv, parameters);
                case CsvType.Discrepancies:
                    return RunQuery<DiscrepancyTimerCsvDto>(Resources.GetDiscrepancyTimersCsv, parameters);
                case CsvType.DowntimeTimers:
                    return RunQuery<DowntimeTimerCsvDto>(Resources.GetDowntimeTimersCsv, parameters);
                case CsvType.EmployeeTimecards:
                    if (departmentID.Length == 1 && departmentID.Contains(DepartmentName.TMCrushing))
                    {
                        return RunQuery<TMCrushingTimecardCsvDto>(Resources.GetEmployeeTimecardCsv, parameters);
                    }
                    return RunQuery<EmployeeTimecardCsvDto>(Resources.GetEmployeeTimecardCsv, parameters);
                case CsvType.TMTimecards:
                    return RunQuery<TMCrushingTimecardCsvDto>(Resources.GetEmployeeTimecardCsv, parameters);
                case CsvType.EquipmentTimers:
                    return RunQuery<EquipmentTimerCsvDto>(Resources.GetEquipmentTimerCsv, parameters);
                case CsvType.JobTimersInvoice:
                    return RunQuery<JobTimersInvoiceCsvDto>(Resources.GetJobTimerCsv, parameters, 5 * 60);
                case CsvType.JobTimers:
                    var departmentsWithEquipmentLabor = new List<DepartmentName?>()
                        {
                            DepartmentName.Concrete,
                            DepartmentName.ConcreteHB,
                            DepartmentName.Concrete2H,
                            DepartmentName.Development,
                            DepartmentName.Residential
                        };
                    if (departmentID.Any(dept => departmentsWithEquipmentLabor.Contains(dept)))
                    {
                        return RunQuery<JobTimerConcreteCsvDto>(Resources.GetJobTimerCsv, parameters);
                    }
                    return RunQuery<JobTimerCsvDto>(Resources.GetJobTimerCsv, parameters);
                case CsvType.EmployeeRoles:
                    return RunQuery<EmployeeRolesCsvDto>(Resources.GetEmployeeRolesCsv, parameters);
                case CsvType.EmployeeClockInsOuts:
                    return RunQuery<EmployeeClockInsOutsCSVDto>(Resources.GetEmployeeClockInsOutsCSV, parameters);
                case CsvType.Notes:
                    return RunQuery<NotesCsvDto>(Resources.GetNotesCsv, parameters);
                case CsvType.Quantities:
                    return RunQuery<QuantitiesCSVDto>(Resources.GetQuantitiesCsv, parameters);
                default:
                    throw new ArgumentException("Csv type not recognized!");
            }
        }

        private IEnumerable<T> RunQuery<T>(string sql, object parameters, int? commandTimeout = null) where T : CsvDto
        {
            using (var sc = new SqlConnection(connectionString))
            {
                return sc.Query<T>(sql, parameters, null, true, commandTimeout);
            }
        }

        public IEnumerable<UnallocatedTimeConcreteDto> GetUnallocatedTimeConcrete(DateTime startDate, DateTime endDate)
        {
            using (var sc = new SqlConnection(connectionString))
            {
                return sc.Query<UnallocatedTimeConcreteDto>(Resources.GetUnallocatedTimeConcreteCsv, new { startDate, endDate });
            }
        }

        public IEnumerable<UnallocatedTimeDto> GetTruckingTimerDiscrepancies(DateTime startDate, DateTime endDate)
        {
            using (var sc = new SqlConnection(connectionString))
            {
                return sc.Query<UnallocatedTimeDto>(Resources.GetTruckingTimerDiscrepanciesCsv, new { startDate, endDate });
            }
        }
    }

    public interface ICsvRepository
    {
        IEnumerable<CsvDto> GetCsv(CsvType type, DateTime startDate, DateTime endDate, DepartmentName[] departmentID);
    }
}
