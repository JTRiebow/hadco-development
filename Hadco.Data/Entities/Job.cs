using Hadco.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DapperAttribute = Dapper.Contrib.Extensions;

namespace Hadco.Data.Entities
{
    //  [Table("Jobs")]
    [DapperAttribute.Table("Jobs")]
    public class Job : IModel
    {
        [Key]
        [Column("JobID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [MaxLength(128)]
        public string JobNumber { get; set; }

        [MaxLength(128)]
        public string Name { get; set; }

        [MaxLength(128)]
        public string Address1 { get; set; }

        [MaxLength(128)]
        public string City { get; set; }

        [MaxLength(128)]
        public string State { get; set; }

        [MaxLength(128)]
        public string Zip { get; set; }

        public Customer Customer { get; set; }
        public int? CustomerID { get; set; }

        [MaxLength(128)]
        public string CustomerNumber { get; set; }

        [MaxLength(128)]
        public string Class { get; set; }

        public DateTime? DateOpen { get; set; }

        public EntityStatus Status { get; set; }

        public string Memo { get; set; }

        public DateTime? DateFiled { get; set; }

        [MaxLength(128)]
        public string PreliminaryFilingNumber { get; set; }

        public int? CustomerTypeID { get; set; }

        public CustomerType CustomerType { get; set; }

        public DateTimeOffset ModifiedDate { get; set; }
    }
}
