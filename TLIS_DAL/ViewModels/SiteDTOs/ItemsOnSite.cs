using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLIS_DAL.ViewModels.SiteDTOs
{
    public class ItemsOnSite
    {
        public int SteelWithLegsCount { get; set; }
        public int SteelWithoutLegs_MastCount { get; set; }
        public int SteelWithoutLegs_MonopoleCount { get; set; }
        public int SteelWithoutLegs_CapsuleCount { get; set; }
        public int NonSteelCount { get; set; }
        public int PowerCount { get; set; }
        public int MW_RFUCount { get; set; }
        public int MW_BUCount { get; set; }
        public int MW_DishCount { get; set; }
        public int MW_ODUCount { get; set; }
        public int MW_OtherCount { get; set; }
        public int RadioAntennaCount { get; set; }
        public int RadioRRUCount { get; set; }
        public int RadioOtherCount { get; set; }
        public int LoadOtherCount { get; set; }
        public int CabinetPowerCount { get; set; }
        public int CabinetTelecomCount { get; set; }
        public int GeneratorCount { get; set; }
        public int SolarCount { get; set; }
        public int SideArmCount { get; set; }
    }
}
