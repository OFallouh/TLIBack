using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TLIS_DAL.Models
{
    public class TLIdependencyRow
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey("TLIdependency")]
        public int? DependencyId { get; set; }
        public TLIdependency Dependency { get; set; }
        [ForeignKey("TLIrow")]
        public int? RowId { get; set; }
        public TLIrow Row { get; set; }
        [ForeignKey("TLIlogicalOperation")]
        public int? LogicalOperationId { get; set; }
        public TLIlogicalOperation LogicalOperation { get; set; }

    }
}
