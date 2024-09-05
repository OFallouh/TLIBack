using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TLIS_DAL.ViewModels.MW_DishDTOs.AddMWDishInstallationObject;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;
using TLIS_DAL.Models;

namespace TLIS_DAL.ViewModels.RadioOtherDTOs
{
    public class AddRadioOtherInstallationObject
    {
        public InstallationRadioOtherConfigObject installationConfig { get; set; }
        public AddCivilLoad civilLoads { get; set; }
        public InstallationRadioOtherAttributeObject installationAttributes { get; set; }
        public List<AddDdynamicAttributeInstallationValueViewModel> dynamicAttribute { get; set; }
        public class InstallationRadioOtherAttributeObject
        {
            public string Name { get; set; }
            public string SerialNumber { get; set; }
            public string? Notes { get; set; } = " ";
            public float HeightBase { get; set; } = 0;
            public float HeightLand { get; set; } = 0;
            public int? ownerId { get; set; }
            public float CenterHigh { get; set; } = 0;
            public float HBA { get; set; } = 0;
            public float HieghFromLand { get; set; } = 0;
            public float EquivalentSpace { get; set; } = 0;
            public float Spaceinstallation { get; set; } = 0;
            public float Azimuth { get; set; } = 0;



        }
        public class InstallationRadioOtherConfigObject
        {
            public int InstallationPlaceId { get; set; }
            public int? civilSteelType { get; set; }
            public int? civilWithLegId { get; set; }
            public int? civilWithoutLegId { get; set; }
            public int? civilNonSteelId { get; set; }
            public int? sideArmId { get; set; }
            public int? legId { get; set; }
            public int radioOtherLibraryId { get; set; }
        }

    }
}