using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModels.CivilLoadsDTOs
{
    public class CivilLoadsViewModel
    {
        public int Id { get; set; }
        public DateTime InstallationDate { get; set; }
        public string ItemOnCivilStatus { get; set; }
        public string ItemStatus { get; set; }
        public bool Dismantle { get; set; }
        public bool ReservedSpace { get; set; }
        public int? sideArmId { get; set; }
        public int allCivilInstId { get; set; }
        public int? allLoadInstId { get; set; }
        public int civilSteelSupportCategoryId { get; set; }
        public string SiteCode { get; set; }

    }
}
