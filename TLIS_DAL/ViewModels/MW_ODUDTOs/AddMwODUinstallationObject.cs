using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TLIS_DAL.ViewModels.MW_DishDTOs.AddMWDishInstallationObject;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;
using TLIS_DAL.Models;

namespace TLIS_DAL.ViewModels.MW_ODUDTOs
{
    public class AddMwODUinstallationObject
    {
        public InstallationMWODUConfigObject installationConfig { get; set; }
        public AddCivilLoad civilLoads { get; set; }
        public installationAttributesMWODU installationAttributes { get; set; }
        public List<AddDdynamicAttributeInstallationValueViewModel> dynamicAttribute { get; set; }
        public class installationAttributesMWODU
        {
            public string Serial_Number { get; set; }
            public string Name { get; set; }
            public string? Notes { get; set; }
            public float Height { get; set; } = 0;
            public ODUConnections? ODUConnections { get; set; }
            public string? Visiable_Status { get; set; }
            public float SpaceInstallation { get; set; } = 0;
            public int? OwnerId { get; set; }
            public float CenterHigh { get; set; } = 0;
            public float HBA { get; set; } = 0;
            public float HieghFromLand { get; set; } = 0;
            public float EquivalentSpace { get; set; } = 0;
            public float Azimuth { get; set; } = 0;
        }
        public class InstallationMWODUConfigObject
        {
            public int InstallationPlaceId { get; set; }
            public int? civilSteelType { get; set; }
            public int? civilWithLegId { get; set; }
            public int? civilWithoutLegId { get; set; }
            public int? civilNonSteelId { get; set; }
            public int sideArmId { get; set; }
            public int MwODULibraryId { get; set; }
            public int Mw_DishId { get; set; }
        }

    }
}
