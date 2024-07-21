using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TLIS_DAL.ViewModels.MW_DishDTOs.AddMWDishInstallationObject;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;
using static TLIS_DAL.ViewModels.MW_BUDTOs.AddMWBUInstallationObject;

namespace TLIS_DAL.ViewModels.MW_BUDTOs
{
    public class EditMWBUInstallationObject
    {
        public EditInstallationMWBUConfigObject installationConfig { get; set; }
        public EditLibraryAttributesMWBUObject civilType { get; set; }
        public AddCivilLoadObjectMWBU civilLoads { get; set; }
        public EditinstallationAttributesMWBUObject installationAttributes { get; set; }
        public List<AddDdynamicAttributeInstallationValueViewModel> dynamicAttribute { get; set; }
        public class EditinstallationAttributesMWBUObject
        {
            public int Id { get; set; }
            public string? Notes { get; set; } = " ";
            public string Name { get; set; }
            public string Serial_Number { get; set; }
            public float Height { get; set; } = 0;
            public float Azimuth { get; set; } = 0;
            public int BUNumber { get; set; } = 0;
            public bool Active { get; set; } = true;
            public string? Visiable_Status { get; set; } = " ";
            public float SpaceInstallation { get; set; } = 0;
            public int? BaseBUId { get; set; }
            public int? OwnerId { get; set; }
            public float CenterHigh { get; set; } = 0;
            public float HBA { get; set; } = 0;
            public float HieghFromLand { get; set; } = 0;
            public float EquivalentSpace { get; set; } = 0;

        }
        public class EditLibraryAttributesMWBUObject
        {
            public int mwBuLibraryId { get; set; }

        }
        public class AddCivilLoadObjectMWBU
        {
            public DateTime InstallationDate { get; set; } = DateTime.Now;
            public string? ItemOnCivilStatus { get; set; } = null;
            public string? ItemStatus { get; set; } = " ";
            public bool ReservedSpace { get; set; }

        }
        public class EditInstallationMWBUConfigObject
        {
            public int InstallationPlaceId { get; set; }
            public int? civilSteelType { get; set; }
            public int? civilWithLegId { get; set; }
            public int? civilWithoutLegId { get; set; }
            public int? civilNonSteelId { get; set; }
            public List<int>? sideArmId { get; set; }
            public int? legId { get; set; }
            public int? sdDishId { get; set; }
            public int mainDishId { get; set; }
            public int? CascededBuId { get; set; }
        }
    }
}
