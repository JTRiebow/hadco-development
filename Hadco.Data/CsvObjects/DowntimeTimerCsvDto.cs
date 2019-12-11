using System;

namespace Hadco.Data.CsvObjects
{
    public class DowntimeTimerCsvDto : CsvDto
    {
        private double _hours;
        public string EmployeeNumber { get; set; }

        public string JobNumber { get; set; }

        public string PhaseNumber { get; set; }

        public string CategoryNumber { get; set; }

        public double Hours
        {
            get { return Math.Round(_hours, 2); }
            set { _hours = value; }
        }

        public string Day { get; set; }

        public string EquipmentNumber { get; set; }

        public string Department { get; set; }
    }
}
