using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TLIS_DAL.Models
{
    public class TLIrule
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public TLIattributeActivated attributeActivated { get; set; }
        public int? attributeActivatedId { get; set; }
        public TLIattributeViewManagment AttributeViewManagment { get; set; }
        public int? AttributeViewManagmentId { get; set; }
        public TLIdynamicAtt dynamicAtt { get; set; }
        public int? dynamicAttId { get; set; }
        [ForeignKey("TLIoperation")]
        public int? OperationId { get; set; }
        public TLIoperation Operation { get; set; }
        public string OperationValueString { get; set; }
        public double? OperationValueDouble { get; set; }
        public DateTime? OperationValueDateTime { get; set; }
        public bool? OperationValueBoolean { get; set; }
        //public string OperationValue { get; set; }
        public TLItablesNames tablesNames { get; set; }
        public int? tablesNamesId { get; set; }
        public bool? IsDynamic { get; set; }
        public IEnumerable<TLIrowRule> rowRules { get; set; }
    }
}
