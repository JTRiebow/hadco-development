using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadco.Data.CsvObjects
{
    public class QuantitiesCSVDto: CsvDto
    {
        public DateTime Day { get; set; }

        public string JobNumber { get; set; }

        public string PhaseNumber { get; set; }

        public string CategoryNumber { get; set; }

        public string UnitsOfMeasure { get; set; }

        public decimal? Est { get; set; }

        public decimal? NewQuantity { get; set; }

        public decimal? PreviousQuantity { get; set; }

        public decimal? OtherNewQuantity { get; set; }

        public decimal? Total { get; set; }
    }
}
