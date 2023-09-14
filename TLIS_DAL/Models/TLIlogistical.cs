using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TLIS_DAL.Models
{
    public class TLIlogistical
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public TLItablePartName tablePartName { get; set; }

        public int tablePartNameId { get; set; }
        public TLIlogisticalType logisticalType { get; set; }
        public int logisticalTypeId { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }

        public IEnumerable<TLIlogisticalitem> logisticalitem { get; set; }
    }
}
