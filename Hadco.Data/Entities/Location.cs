using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NUnit.Framework.Constraints;

namespace Hadco.Data.Entities
{
    [Table("Locations")]
    public class Location : IModel
    {
        [Key]
        [Column("LocationID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public int EmployeeTimerID { get; set; }

        public EmployeeTimer EmployeeTimer { get; set; }

        public DateTimeOffset TimeStamp { get; set; }

        public decimal Latitude { get; set; }

        public decimal Longitude { get; set; }

    }
}
