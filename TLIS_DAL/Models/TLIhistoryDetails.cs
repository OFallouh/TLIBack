using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TLIS_DAL.Models
{
    public enum AttributeType
    {
        Dynamic = 0,
        Static = 1
    }
    public class TLIhistoryDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int? TablesHistoryId { get; set; }
        public TLItablesHistory TablesHistory { get; set; }
        public int? WorkflowTableHistoryId { get; set; }
        public TLIworkflowTableHistory WorkflowTableHistory { get; set; }
        [Required]
        public string AttName { get; set; }
        
        public string OldValue { get; set; }
        [Required]
        public string NewValue { get; set; }
        public AttributeType AttributeType { get; set; }
       // public string AttType { get; set; }
    }
}
