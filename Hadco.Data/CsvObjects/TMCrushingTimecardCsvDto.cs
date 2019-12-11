using System;

namespace Hadco.Data.CsvObjects
{
    public class TMCrushingTimecardCsvDto : CsvDto
    {
        private double _totalHours;
        public string Employee { get; set; }

        public string Day { get; set; }

        public double TotalHours
        {
            get { return Math.Round(_totalHours, 2); }
            set { _totalHours = value; }
        }

        public string Pit { get; set; }
    }
}