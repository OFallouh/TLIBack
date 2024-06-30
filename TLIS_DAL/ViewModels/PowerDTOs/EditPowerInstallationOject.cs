using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TLIS_DAL.ViewModels.MW_DishDTOs.AddMWDishInstallationObject;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;

namespace TLIS_DAL.ViewModels.PowerDTOs
{
    public class EditPowerInstallationOject
    {
        public EditInstallationPowerConfigObject installationConfig { get; set; }
        public AddCivilLoad civilLoads { get; set; }
        public LibraryAttributesPowerObject civilType { get; set; }
        public EditInstallationPowerAttributeObject installationAttributes { get; set; }
        public List<AddDdynamicAttributeInstallationValueViewModel> dynamicAttribute { get; set; }
        public class LibraryAttributesPowerObject
        {
            public int powerLibraryId { get; set; }

        }
        public class EditInstallationPowerAttributeObject
        {
            public int Id { get; set; }
            public string Name { get; set; } = "";
            public float HeightBase { get; set; } = 0;
            public float HeightLand { get; set; } = 0;
            public float SpaceInstallation { get; set; } = 0;
            public float Azimuth { get; set; } = 0;
            public float Height { get; set; } = 0;
            public string? Notes { get; set; } = "";
            public int? ownerId { get; set; } = null;
            public int? powerTypeId { get; set; } = null;
            public string? VisibleStatus { get; set; } = "";
            public float CenterHigh { get; set; } = 0;
            public float HBA { get; set; } = 0;
            public float HieghFromLand { get; set; } = 0;
            public float EquivalentSpace { get; set; } = 0;


        }
        public class EditInstallationPowerConfigObject
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