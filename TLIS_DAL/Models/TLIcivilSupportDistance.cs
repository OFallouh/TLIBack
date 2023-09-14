using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TLIS_DAL.Models
{
    
    public class TLIcivilSupportDistance
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public float? Distance { get; set; }
        public float? Azimuth { get; set; }
        public virtual TLIallCivilInst CivilInst { get; set; }
        [Required]
        public int CivilInstId { get; set; }
       // public virtual TLIallCivilInst ReferenceCivil { get; set; }
       [Required]
        public int ReferenceCivilId { get; set; }
        public TLIsite site { get; set; }
        [Required]
        public string SiteCode { get; set; }
    }
}
