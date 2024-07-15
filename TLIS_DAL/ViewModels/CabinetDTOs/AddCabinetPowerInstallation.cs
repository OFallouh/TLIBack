using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;
using TLIS_DAL.ViewModels.OtherInSiteDTOs;
using TLIS_DAL.ViewModels.OtherInventoryDistanceDTOs;

namespace TLIS_DAL.ViewModels.CabinetDTOs
{
    public class AddCabinetPowerInstallation
    {
        public LibraryAttributesCabinetPower CabinetPowerType { get; set; }
        public installationAttributesCabinetPower installationAttributes { get; set; }
        public AddOtherInSiteViewModel OtherInSite { get; set; }
        public AddOtherInventoryDistanceViewModel OtherInventoryDistance { get; set; }
        public List<AddDdynamicAttributeInstallationValueViewModel> dynamicAttribute { get; set; }
        public class LibraryAttributesCabinetPower
        {
            public int CabinetPowerLibraryId { get; set; }
        }

        public class installationAttributesCabinetPower
        {
            public string Name { get; set; }
            public string? TPVersion { get; set; } = "";
            public int RenewableCabinetNumberOfBatteries { get; set; } = 0;
            public int NUmberOfPSU { get; set; } = 0;
            public float SpaceInstallation { get; set; } = 0;
            public string? VisibleStatus { get; set; } = " ";
            public int? RenewableCabinetTypeId { get; set; }
            public float CenterHigh { get; set; } = 0;
            public float HBA { get; set; } = 0;
            public float HieghFromLand { get; set; } = 0;
            public float EquivalentSpace { get; set; } = 0;

        }
    }
}
