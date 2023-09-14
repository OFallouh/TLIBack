using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModels.CivilLoadsDTOs
{
    public class AddCivilLoadsViewModel
    {
        public DateTime InstallationDate { get; set; }
        public string ItemOnCivilStatus { get; set; }
        public string ItemStatus { get; set; }
        public bool Dismantle { get; set; }
        public int? legId { get; set; }
        public int? Leg2Id { get; set; }
        public bool ReservedSpace { get; set; } = false;
        public int? sideArmId { get; set; }
        public int allCivilInstId { get; set; }
        public int civilSteelSupportCategoryId { get; set; }
       
    }
}
