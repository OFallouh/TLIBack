using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;

namespace TLIS_DAL.ViewModels.Mw_OtherDTOs
{
    public class AddMWOtherInstallationObject
    {
        public InstallationMWOtherConfigObject installationConfig { get; set; }
        public AddcivilloadMWother civilLoads { get; set; }
        public installationAttributesMWOther installationAttributes { get; set; }
        public List<AddDdynamicAttributeInstallationValueViewModel> dynamicAttribute { get; set; }
        public class installationAttributesMWOther
        {
            public string Name { get; set; }
            public string SerialNumber { get; set; }
            public string? Notes { get; set; } = " ";
            public float HeightBase { get; set; } = 0;
            public float HeightLand { get; set; } = 0;
            public string? VisibleStatus { get; set; } = " ";
            public float CenterHigh { get; set; } = 0;
            public float HBA { get; set; } = 0;
            public float HieghFromLand { get; set; } = 0;
            public float EquivalentSpace { get; set; } = 0;
            public float Azimuth { get; set; } = 0;
            public float Spaceinstallation { get; set; } = 0;


        }
        public class InstallationMWOtherConfigObject
        {
            public int InstallationPlaceId { get; set; }
            public int? civilSteelType { get; set; }
            public int? civilWithLegId { get; set; }
            public int? civilWithoutLegId { get; set; }
            public int? civilNonSteelId { get; set; }
            public int? sideArmId { get; set; }
            public int? legId { get; set; }
            public int mwOtherLibraryId { get; set; }
        }
        public class AddcivilloadMWother
        {
            public DateTime InstallationDate { get; set; } = DateTime.Now;
            public string? ItemOnCivilStatus { get; set; } = null;
            public string? ItemStatus { get; set; } = " ";
            public bool ReservedSpace { get; set; }

        }

    }
}

