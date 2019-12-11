namespace Hadco.Common.DataTransferObjects
{
    public class EmployeeTimecardSummaryDto
    {
        public int EmployeeID { get; set; }

        public string Day { get; set; }

        public double TotalHours { get; set; }
        public int TotalMinutes { get; set; }
    }
}
