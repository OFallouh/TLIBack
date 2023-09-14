using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TLIS_DAL.Models
{
    public class TLIdynamicAttLibValue
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Value { get; set; }
        public string ValueString { get; set; }
        public double? ValueDouble { get; set; }
        public DateTime? ValueDateTime { get; set; }
        public bool? ValueBoolean { get; set; }
        [ForeignKey("TLIdynamicAtt")]
        public int DynamicAttId { get; set; }
        public TLIdynamicAtt DynamicAtt { get; set; }
        public bool disable { get; set; }
        public TLItablesNames tablesNames { get; set; }
        public int tablesNamesId { get; set; }
        public int InventoryId { get; set; }
        public TLIdynamicListValues dynamicListValues { get; set; }
        public int? dynamicListValuesId { get; set; }
    }
}
