
using System;

namespace Hadco.Data.CsvObjects
{

    //Concrete, Development and Residential- Job Timers(All Employees in CDR)
    //EmployeeNumber(P.SAMA), Day, JobNumber, PhaseNumber, CategoryNumber, Labor Hours, EquipmentNumber, Equipcode (r, re, etc.), EquipmentHours, Total Hours, Department

    public class JobTimerConcreteCsvDto : CsvDto
    {
        private double _laborHours;
        private double _totalHours;
        private double _equipmentHours;

        // Employee will be Employee Number unless null. Then it will be Employee Username.
        public string Employee { get; set; }

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

        public string Department { get; set; }

    }
}