﻿using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Policy;

namespace Hadco.Common.Enums
{
    /// <summary>
    /// Each value must be unique.
    /// For any new value, add it to the bottom of the list with the value incremented
    /// from the previous bottom entry.
    /// </summary>
    public enum AuthActivityID
    {
        EditPermissions = 0,
        ClockInFromWeb = 1,
        ViewTimeCard = 2,
        ViewAccountingTimers = 3,
        ViewBillingTimers = 4,
        ViewSupervisorTimers = 5,
        ViewForemenTimesheets = 6,
        ViewEmployeeTimesheets = 7,
        AddEmployeeTimer = 8,
        ViewEmployeeTimer = 9,
        ApproveEmployeeTimer = 10,
        FlagEmployeeTimer = 11,
        EditEmployeeTimerEntry = 12,
        DeleteEmployeeTimerEntry = 13,
        AddEmployeeTimerEntry = 14,
        ViewEmployeeTimerEditHistory = 15,
        ViewEmployeeOccurrences = 16,
        EditEmployeeOccurrences = 17,
        DeleteEmployeeOccurrences = 18,
        AddTimersFromSupervisorCard = 19,
        EditTimerOverhead = 20,
        ViewTimerOverheadEditHistory = 21,
        ApproveEmployeeTimesheet = 22,
        RejectEmployeeTimesheet = 23,
        EditOdometerReading = 24,
        ResolveEmployeeTimerFlag = 25,
        MarkTimerInjury = 26,
        AddForemanTimesheet = 27,
        ViewForemanTimesheet = 28,
        EditForemanTimer = 29,
        ViewForemanTimeCard = 30,
        ApproveForemanTimer = 31,
        FlagForemanTimer = 32,
        RejectForemanTimer = 33,
        EditForemanTimerJob = 34,
        DeleteForemanTimerJob = 35,
        AddForemanTimerJob = 36,
        AddForemanTimerJobEquipment = 37,
        DeleteForemanTimerJobEquipment = 38,
        AddForemanTimerOccurrence = 39,
        EditForemanTimerOccurrence = 40,
        DeleteForemanTimerOccurrence = 41,
        ViewTruckerDailies = 42,
        EditTruckerDaily = 43,
        ExportTruckerDailiesAsCsv = 44,
        ExportVisibleTruckerDailiesAsCsv = 45,
        ViewTruckingPricing = 46,
        ViewTruckingReporting = 47,
        ViewTMCrushing = 48,
        EditProductionItem = 49,
        AddProductionItem = 50,
        DeleteProductionItem = 51,
        EditDowntimeItem = 52,
        AddDowntimeItem = 53,
        DeleteDowntimeItem = 54,
        AddCrushingReportNotes = 55,
        SubmitCrushingReport = 56,
        ViewDowntimeReasons = 57,
        EditDowntimeReasons = 58,
        DeleteDowntimeReason = 59,
        CreateDowntimeReason = 60,
        ViewGpsSettings = 61,
        ViewJobs = 62,
        ViewMaterials = 63,
        ViewOccurrences = 64,
        ViewTruckingClassifications = 65,
        EditMaterial = 66,
        DeleteMaterial = 67,
        AddMaterial = 68,
        EditOccurrence = 69,
        DeleteOccurrence = 70,
        AddOccurrence = 71,
        ViewEmployeeList = 72,
        CreateEmployee = 73,
        ViewSupervisors = 74,
        ViewEmployeeDetails = 75,
        ViewCurrentEmployeeDetails = 76,
        AddEmployeeRole = 77,
        DeleteEmployeeRole = 78,
        AddEmployeeDepartment = 79,
        DeleteEmployeeDepartment = 80,
        AddEmployeeSupervisor = 81,
        DeleteEmployeeSupervisor = 82,
        ChangeEmployeePassword = 83,
        ChangeEmployeePin = 84,
        ChangeEmployeeUsername = 85,
        ChangeEmployeeName = 86,
        ChangeEmployeePhone = 87,
        DownloadJobTimersCsv = 88,
        DownloadOccurrencesCsv = 89,
        DownloadEmployeeTimecardsCsv = 90,
        DownloadDiscrepanciesCsv = 91,
        DownloadLoadTimersCsv = 92,
        DownloadDowntimeCsv = 93,
        DownloadEquipmentTimersCsv = 94,
        DownloadJobTimersWithEquipmentCsv = 95,
        EditTimerNote = 96,
        RejectSupervisorApproval = 97,
        RejectBillingApproval = 98,
        RejectAccountingApproval = 99,
        ApproveAsSupervisor = 100,
        ApproveAsBilling = 101,
        ApproveAsAccounting = 102,
        SearchEmployees = 103,
        DownloadEmployeeRolesCsv = 104,
        DownloadEmployeeClockInsOutsCSV = 105,
        DownloadNotesCSV = 106,
        DownloadQuantitiesCSV = 107,
    }
}
