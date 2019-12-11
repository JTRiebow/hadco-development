using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hadco.Data.Entities
{
    [Table("Pits")]
    public class Pit : IModel
    {
        [Key]
        [Column("PitID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [MaxLength(128)]
        [Required]
        [Index(IsUnique = true)]
        public string Name { get; set; }
    }
}
