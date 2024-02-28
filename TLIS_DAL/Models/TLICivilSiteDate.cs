using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TLIS_DAL.Models
{
    public class TLIcivilSiteDate
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public float LongitudinalSpindleLengthm { get; set; }
        public float HorizontalSpindleLengthm { get; set; }
        public DateTime InstallationDate { get; set; }
        public TLIallCivilInst allCivilInst { get; set; }
        [Required]
        public int allCivilInstId { get; set; }
        [ForeignKey("TLIsite")]
        [Required]
        public string SiteCode { get; set; }
        public TLIsite Site { get; set; }
        public bool ReservedSpace { get; set; }
        public bool Dismantle { get; set; }
    }
}
