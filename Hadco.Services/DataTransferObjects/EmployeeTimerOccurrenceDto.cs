using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Hadco.Services.DataTransferObjects
{
    public class EmployeeTimerOccurrenceDto
    {
        [Required]
        public int OccurrenceID { get; set; }
    }
}
