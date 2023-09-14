using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TLIS_DAL.ViewModels.OtherInSiteDTOs
{
    public class OtherInSiteViewModel
    {
        public int Id { get; set; }
        public string OtherInSiteStatus { get; set; }
        public string OtherInventoryStatus { get; set; }
        public DateTime InstallationDate { get; set; }
        public string SiteCode { get; set; }
        public string SiteName { get; set; }
        public int allOtherInventoryInstId { get; set; }
        public bool ReservedSpace { get; set; }
        public bool Dismantle { get; set; }
        
    }
}
