using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TLIS_DAL.Models
{
    public class TLIotherInSite
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? OtherInSiteStatus { get; set; }
        public string? OtherInventoryStatus { get; set; }
        public DateTime InstallationDate { get; set; }
        public TLIsite Site { get; set; }
        public string SiteCode { get; set; }
        public TLIallOtherInventoryInst allOtherInventoryInst { get; set; }
        public int allOtherInventoryInstId { get; set; }
        public bool ReservedSpace { get; set; }
        public bool Dismantle { get; set; }
      
    }
}
