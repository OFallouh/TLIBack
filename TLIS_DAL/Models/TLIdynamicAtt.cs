using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TLIS_DAL.Models
{
    public class TLIdynamicAtt
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Key { get; set; }
        public bool LibraryAtt { get; set; }
        public int? DataTypeId { get; set; }
        public TLIdataType DataType { get; set; }
        public string Description { get; set; }
        public int? CivilWithoutLegCategoryId { get; set; }
        public TLIcivilWithoutLegCategory CivilWithoutLegCategory { get; set; }
        public TLItablesNames tablesNames { get; set; }
        public int tablesNamesId { get; set; }
        public bool Required { get; set; }
        public bool disable { get; set; }
        public string ? DefaultValue { get; set; }
        //public bool IsResult { get; set; }
        public IEnumerable<TLIdynamicListValues> dynamicListValues { get; set; }
        public IEnumerable<TLIdynamicAttInstValue> dynamicAttInstValues { get; set; }
        public IEnumerable<TLIdynamicAttLibValue> dynamicAttLibValues { get; set; }
        public IEnumerable<TLIdependency> dependencies { get; set; }
        public IEnumerable<TLIvalidation> validations { get; set; }
        public IEnumerable<TLIrule> rules { get; set; }
        public IEnumerable<TLIattributeViewManagment> AttributeViewManagments { get; set; }
    }
}
