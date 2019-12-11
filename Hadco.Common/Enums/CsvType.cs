using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Hadco.Common.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum CsvType
    {
        LoadTimers,
        Occurrences,
        JobTimers,
        JobTimerConcrete,
        JobTimerCdr,
        JobTimersInvoice,
        Discrepancies,
        DowntimeTimers,
        EmployeeTimecards,
        TMTimecards,
        EquipmentTimers,
        EmployeeRoles,
        EmployeeClockInsOuts,
        Notes,
        Quantities,
    }
}
