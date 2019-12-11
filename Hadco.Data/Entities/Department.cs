using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hadco.Common.Enums;

namespace Hadco.Data.Entities
{
    [Table("Departments")]
    public class Department : IModel
    {
        [Key]
        [Column("DepartmentID")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID { get; set; }

        [MaxLength(128)]
        [Required]
        [Index(IsUnique = true)]
        public string Name { get; set; }

        public int AuthenticationTimeoutMinutes { get; set; }

        public virtual ICollection<Employee> Employees { get; set; }

        public virtual TrackedByType? TrackedBy { get; set; }
    }
}
