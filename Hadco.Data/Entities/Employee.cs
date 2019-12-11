using Hadco.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Hadco.Data.Entities
{
    [Table("Employees")]
    public class Employee : TrackedEntity, IModel
    {
        [Key]
        [Column("EmployeeID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [MaxLength(64)]
        [Required]
        [Index(IsUnique = true)]
        public string Username { get; set; }

        [Required]
        [MaxLength(256)]
        public string Password { get; set; }

        [MaxLength(128)]
        public string EmployeeNumber { get; set; }

        [MaxLength(128)]
        public string Name { get; set; }

        [MaxLength(128)]
        public string Phone { get; set; }

        [MaxLength(8)]
        public string Pin { get; set; }

        public bool OriginComputerEase { get; set; }

        public int EmployeeTypeID { get; set; }
        public EmployeeType EmployeeType { get; set; }

        public EntityStatus Status { get; set; }   
        [MaxLength(32)]
        public string Location { get; set; }     

        public virtual ICollection<Employee> Employees { get; set; }

        public virtual ICollection<Employee> Supervisors { get; set; }

        public virtual ICollection<Department> Departments { get; set; }

        public virtual ICollection<Role> Roles { get; set; }

        public ClockedInStatus ClockedInStatus { get; set; }
        
        public DateTimeOffset? LastAuthenticatedOn { get; set; }
    }
}
