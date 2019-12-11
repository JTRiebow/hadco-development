using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hadco.Common.Enums;

namespace Hadco.Data.Entities
{
    [Table("AuthSections")]
    public class AuthSection
    {
        [Key]
        [Column("AuthSectionID")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public AuthSectionID ID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

    }
}
