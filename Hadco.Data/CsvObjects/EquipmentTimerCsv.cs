using System;

namespace Hadco.Data.CsvObjects
{
    public class EquipmentTimerCsvDto : CsvDto
    {
        private double _totalHours;
        public string EmployeeNumber { get; set; }

        public string Day { get; set; }

        public double TotalHours
        {
            get { return Math.Round(_totalHours, 2); }
            set { _totalHours = value; }
        }

        public string EquipmentNumber { get; set; }

        public string EquipmentServiceType { get; set; }
    }
}