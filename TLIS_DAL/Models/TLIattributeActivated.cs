using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TLIS_DAL.Models
{
    public class TLIattributeActivated
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Key { get; set; }
        public string Label { get; set; }
        public string Tabel { get; set; }
        public string Description { get; set; }
        public bool Required { get; set; }
        public bool Manage { get; set; }
        public bool enable { get; set; }
        public bool AutoFill { get; set; }
        public string DataType { get; set; }
        public IEnumerable<TLIattActivatedCategory> activatedCategories { get; set; }
        public IEnumerable<TLIrule> rules { get; set; }
        public IEnumerable<TLIattributeViewManagment> AttributeViewManagments { get; set; }
    }
}
