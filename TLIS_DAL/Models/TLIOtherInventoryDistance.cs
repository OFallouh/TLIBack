using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TLIS_DAL.Models
{
    public class TLIotherInventoryDistance
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        public float Distance { get; set; }
        public float Azimuth { get; set; }
        public TLIallOtherInventoryInst allOtherInventoryInst { get; set; }
        public int allOtherInventoryInstId { get; set; }
        public int? ReferenceOtherInventoryId { get; set; }
        public TLIsite site { get; set; }
        public string SiteCode { get; set; }
        //public TLIotherInventoryType otherInventoryType2 { get; set; }
        //public int? otherInventoryTypeID2 { get; set; }
    }
}
