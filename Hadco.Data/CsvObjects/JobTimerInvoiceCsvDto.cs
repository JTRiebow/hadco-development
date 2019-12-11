using System;

namespace Hadco.Data.CsvObjects
{

    
//Concrete, Development and Residential- Invoice Timers(All Employees in CDR)
//EmployeeNumber(P.SAMA), Name, Day, JobNumber, PhaseNumber, CategoryNumber, Labor Hours, EquipmentNumber, Equipcode (r, re, etc.), EquipmentHours, Total Hours, Department, Invoice Number

    public class JobTimersInvoiceCsvDto : CsvDto
    {
        private double _laborHours;
        private double _equipmentHours;
        private double _totalHours;

        // Employee will be Employee Number unless null. Then it will be Employee Username.
        public string Employee { get; set; }

        public string Name { get; set; }

        public string Day { get; set; }

        public string JobNumber { get; set; }

        public string PhaseNumber { get; set; }

        public string CategoryNumber { get; set; }

        public double LaborHours
        {
            get { return Math.Round(_laborHours, 2); }
            set { _laborHours = value; }
        }

        public string EquipmentNumber { get; set; }

        public string EquipmentCode { get; set; }

        public double EquipmentHours
        {
            get { return Math.Round(_equipmentHours, 2); }
            set { _equipmentHours = value; }
        }

        public double TotalHours
        {
            get { return Math.Round(_totalHours, 2); }
            set { _totalHours = value; }
        }

        public string Department => "FIELD WAGE";

        public string InvoiceNumber { get; set; }

        public string JobDiaryNote { get; set; }
    }
}
