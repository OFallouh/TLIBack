using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TLIS_DAL.Models
{
    public class TLIdynamicListValues
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Value { get; set; }
        public TLIdynamicAtt dynamicAtt { get; set; }
        public int dynamicAttId { get; set; }
        public bool Delete { get; set; }
        public bool Disable { get; set; }
        public IEnumerable<TLIdynamicAttInstValue> dynamicAttInstValues { get; set; }
        public IEnumerable<TLIdynamicAttLibValue> dynamicAttLibValues { get; set; }
    }
}
