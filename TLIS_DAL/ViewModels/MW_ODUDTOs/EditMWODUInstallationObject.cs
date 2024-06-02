using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TLIS_DAL.ViewModels.MW_DishDTOs.AddMWDishInstallationObject;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;
using static TLIS_DAL.ViewModels.MW_ODUDTOs.AddMwODUinstallationObject;

namespace TLIS_DAL.ViewModels.MW_ODUDTOs
{
    public class EditMWODUInstallationObject
    {
        public InstallationMWODUConfigObject installationConfig { get; set; }
        public LibraryAttributesMWODUObject civilType { get; set; }
        public EditinstallationAttributesMWODUObject installationAttributes { get; set; }
        public AddCivilLoad civilLoads { get; set; }
        public List<AddDdynamicAttributeInstallationValueViewModel> dynamicAttribute { get; set; }
        public class EditinstallationAttributesMWODUObject
        {
            public int Id { get; set; }
            public string Serial_Number { get; set; }
            public string? Notes { get; set; }
            public float Height { get; set; } = 0;
            public string? Visiable_Status { get; set; }
            public float SpaceInstallation { get; set; } = 0;
            public int? OwnerId { get; set; }
            public float CenterHigh { get; set; } = 0;
            public float HBA { get; set; } = 0;
            public float HieghFromLand { get; set; } = 0;
            public float Azimuth { get; set; } = 0;

        }
        public class LibraryAttributesMWODUObject
        {
            public int MwODULibraryId { get; set; }

        }
       
    }
}
