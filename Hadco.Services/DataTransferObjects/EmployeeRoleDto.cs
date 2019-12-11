using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Hadco.Services.DataTransferObjects
{
    public class EmployeeRoleDto
    {
        [Required]
        public int RoleID { get; set; }
    }
}
