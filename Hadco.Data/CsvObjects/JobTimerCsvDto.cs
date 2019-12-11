using System;

namespace Hadco.Data.CsvObjects
{
    public class JobTimerCsvDto : CsvDto
    {
        private double _totalHours;

        // Employee will be Employee Number unless null. Then it will be Employee Username.
        public string Employee { get; set; }

        public string Day { get; set; }

        public double TotalHours
        {
            get { return Math.Round(_totalHours, 2); }
            set { _totalHours = value; }
        }

        public string JobNumber { get; set; }

        public string PhaseNumber { get; set; }

        public string CategoryNumber { get; set; }

        public string Department => "FIELD WAGE";
    }
}
