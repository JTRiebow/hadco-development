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
    [Table("AuthActivities")]
    public class AuthActivity : IModel
    {
        [Key]
        [Column("AuthActivityID")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID { get; set; }

        public AuthSectionID AuthSectionID { get; set; }
        public virtual AuthSection AuthSection { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
        
        public string Key { get; set; }
    }
}
