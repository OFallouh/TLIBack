using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TLIS_DAL.ViewModels.MW_DishDTOs.AddMWDishInstallationObject;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;
using static TLIS_DAL.ViewModels.RadioAntennaDTOs.AddRadioAntennaInstallationObject;

namespace TLIS_DAL.ViewModels.RadioAntennaDTOs
{
    public class EditRadioAntennaInstallationObject
    {
        public EditInstallationRadioAntennaConfigObject installationConfig { get; set; }
        public AddCivilLoad civilLoads { get; set; }
        public LibraryAttributesRadioAntennaObject civilType { get; set; }
        public EditInstallationRadioAntennaAttributeObject installationAttributes { get; set; }
        public List<AddDdynamicAttributeInstallationValueViewModel> dynamicAttribute { get; set; }
        public class LibraryAttributesRadioAntennaObject
        {
            public int radioAntennaLibraryId { get; set; }

        }
        public class EditInstallationRadioAntennaAttributeObject
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public float Azimuth { get; set; } = 0;
            public float MechanicalTilt { get; set; } = 0;
            public float ElectricalTilt { get; set; } = 0;
            public string SerialNumber { get; set; }
            public float HBASurface { get; set; } = 0;
            public string? Notes { get; set; } = "";
            public float HeightBase { get; set; } = 0;
            public float HeightLand { get; set; } = 0;
            public float SpaceInstallation { get; set; } = 0;
            public string? VisibleStatus { get; set; } = "";
            public TLIowner owner { get; set; }
            public int? ownerId { get; set; }
            public float CenterHigh { get; set; } = 0;
            public float HBA { get; set; } = 0;
            public float HieghFromLand { get; set; } = 0;
            public float EquivalentSpace { get; set; } = 0;


        }
        public class EditInstallationRadioAntennaConfigObject
        {
            public int InstallationPlaceId { get; set; }
            public int? civilSteelType { get; set; }
            public int? civilWithLegId { get; set; }
            public int? civilWithoutLegId { get; set; }
            public int? civilNonSteelId { get; set; }
            public int? sideArmId { get; set; }
            public int? legId { get; set; }
        }

    }
}