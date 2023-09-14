using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TLIS_DAL.Models
{
    public class TLIlogisticalitem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public int RecordId { get; set; }
        public virtual TLItablesNames tablesNames { get; set; }
        public int tablesNamesId { get; set; }
        public bool IsLib { get; set; }
        public int? logisticalId { get; set; }
        public virtual TLIlogistical logistical { get; set; }
        
    }
}
