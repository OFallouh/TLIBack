using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TLIS_DAL.Models
{
    public class TLIdependency
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey("TLIdynamicAtt")]
        public int? DynamicAttId { get; set; }
        public TLIdynamicAtt DynamicAtt { get; set; }
        [ForeignKey("TLIoperation")]
        public int? OperationId { get; set; }
        public TLIoperation Operation { get; set; }
        public string ValueString { get; set; }
        public double? ValueDouble { get; set; }
        public DateTime? ValueDateTime { get; set; }
        public bool? ValueBoolean { get; set; }
        //public string Value { get; set; }
        public bool IsResult { get; set; }
        public List<TLIdependencyRow> DependencyRows { get; set; }
    }
}
