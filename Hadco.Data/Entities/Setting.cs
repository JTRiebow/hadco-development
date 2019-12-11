using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadco.Data.Entities
{
    [Table("Settings")]
    public class Setting : IModel
    {
        [Column("SettingID")]
        public int ID { get; set; }
        public int BreadCrumbSeconds { get; set; }
        public DateTimeOffset ModifiedTime { get; set; }
    }
}
