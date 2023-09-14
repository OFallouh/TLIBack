 using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TLIS_DAL.Models
{
    public class TLIrowRule
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }       
        [ForeignKey("TLIrule")]
        public int? RuleId { get; set; }
        public TLIrule Rule { get; set; }
        [ForeignKey("TLIrow")]
        public int? RowId { get; set; }
        public TLIrow Row { get; set; }
        [ForeignKey("TLIlogicalOperation")]
        public int? LogicalOperationId { get; set; }
        public TLIlogicalOperation LogicalOperation { get; set; }

    }
}
