using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Toolbelt.ComponentModel.DataAnnotations.Schema;

namespace TLIS_DAL.Models
{
    public class TLItablesNames
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [Index(IsUnique = true)]
        public string TableName { get; set; }
        public TLItablePartName tablePartName { get; set; }
        public int? tablePartNameId { get; set; }
        public bool IsEquip { get; set; }
        public IEnumerable<TLIdynamicAtt> dynamicAtts { get; set; }
        public IEnumerable<TLIattachedFiles> attachedFiles { get; set; }
        public IEnumerable<TLIrule> rules { get; set; }
        public IEnumerable<TLIdynamicAttInstValue> dynamicAttInstValues { get; set; }
        public IEnumerable<TLIdynamicAttLibValue> dynamicAttLibValues { get; set; }
        public IEnumerable <TLIlogisticalitem> logisticalitem { get; set; }
    }
}
