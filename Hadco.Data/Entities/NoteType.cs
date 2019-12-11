using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadco.Data.Entities
{
    [Table("NoteTypes")]
    public class NoteType : IModel
    {
        [Column("NoteTypeID")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsSystemGenerated { get; set; }
    }
}
