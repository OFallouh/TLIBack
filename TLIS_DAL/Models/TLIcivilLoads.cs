using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TLIS_DAL.Models
{
    public class TLIcivilLoads
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateTime InstallationDate { get; set; }
        //remove
        public string ItemOnCivilStatus { get; set; }
        public string ItemStatus { get; set; }
        public bool Dismantle { get; set; }
        //
        public TLIleg leg { get; set; }
        public int ? legId { get; set; }
        public int ? Leg2Id { get; set; }
        public bool ReservedSpace { get; set; }
        public TLIsideArm sideArm { get; set; }
        public int? sideArmId { get; set; }
        public int? sideArm2Id { get; set; }
        public TLIallCivilInst allCivilInst { get; set; }
        public int allCivilInstId { get; set; }
        public TLIallLoadInst allLoadInst { get; set; }
        public int? allLoadInstId { get; set; }
        public TLIcivilSteelSupportCategory? civilSteelSupportCategory { get; set; }
        public int? civilSteelSupportCategoryId { get; set; }
        public TLIsite site { get; set; }
        public string SiteCode { get; set; }
        public IEnumerable<TLIcivilLoadLegs> civilLoads { get; set; }
        
    }
}
