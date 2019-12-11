using System;

namespace Hadco.Data.CsvObjects
{
    public class LoadTimerCsvDto : CsvDto
    {
        private double _hours;
        private double _equipmentHours;
        public string EmployeeNumber { get; set; }

        public string Class { get; set; }

        public string JobNumber { get; set; }

        public string PhaseNumber { get; set; }

        public string CategoryNumber { get; set; }

        public double Hours
        {
            get { return Math.Round(_hours, 2); }
            set { _hours = value; }
        }

        public string Units { get; set; }

        public string Day { get; set; }

        public string EquipmentNumber { get; set; }

        public string EquipmentCode { get; set; }

        public double EquipmentHours
        {
            get { return Math.Round(_equipmentHours, 2); }
            set { _equipmentHours = value; }
        }

        public string Department { get; set; }
    }
}