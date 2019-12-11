using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadco.Data.CsvObjects
{
    class DiscrepancyTimerCsvDto : CsvDto
    {
        private double _travelHours;
        private double _greaseHours;
        private double _dailyHours;
        private double _shopHours;
        private double _laborHours;
        private double _equipmentHours;
        private double _employeeHours;
        private double _totalHours;
        private double _hoursDifference;

        public string Name { get; set; }

        // Employee will be Employee Number unless null. Then it will be Employee Username.
        public string Employee { get; set; }

        public string Day { get; set; }

        public double EmployeeHours
        {
            get { return Math.Round(_employeeHours, 2); }
            set { _employeeHours = value; }
        }

        public double TotalHours
        {
            get { return Math.Round(_totalHours, 2); }
            set { _totalHours = value; }
        }

        public string Department { get; set; }

        public double TravelHours
        {
            get { return Math.Round(_travelHours, 2); }
            set { _travelHours = value; }
        }

        public double GreaseHours
        {
            get { return Math.Round(_greaseHours, 2); }
            set { _greaseHours = value; }
        }

        public double DailyHours
        {
            get { return Math.Round(_dailyHours, 2); }
            set { _dailyHours = value; }
        }

        public double ShopHours
        {
            get { return Math.Round(_shopHours, 2); }
            set { _shopHours = value; }
        }

        public double LaborHours
        {
            get { return Math.Round(_laborHours, 2); }
            set { _laborHours = value; }
        }

        public double EquipmentHours
        {
            get { return Math.Round(_equipmentHours, 2); }
            set { _equipmentHours = value; }
        }

        public double HoursDifference
        {
            get { return Math.Round(_hoursDifference, 2); }
            set { _hoursDifference = value; }
        }
    }
}
