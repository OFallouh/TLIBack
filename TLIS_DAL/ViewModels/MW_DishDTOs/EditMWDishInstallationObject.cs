using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;

namespace TLIS_DAL.ViewModels.MW_DishDTOs
{
    public class EditMWDishInstallationObject
    {
        public LibraryAttributesMWDish civilType { get; set; }
        public AddCivilLoad civilLoads { get; set; }
        public installationAttributesMWDish installationAttributes { get; set; }
        public List<AddDdynamicAttributeInstallationValueViewModel> dynamicAttribute { get; set; }
        public class installationAttributesMWDish
        {
            public int Id { get; set; } 
            public float Azimuth { get; set; } = 0;
            public string? Notes { get; set; } = " ";
            public string? Far_End_Site_Code { get; set; } = " ";
            public float HBA_Surface { get; set; } = 0;
            public string Serial_Number { get; set; }
            public string DishName { get; set; }
            public int? MW_LinkId { get; set; }
            public string? Visiable_Status { get; set; } = " ";
            public float SpaceInstallation { get; set; } = 0;
            public float HeightBase { get; set; } = 0;
            public float HeightLand { get; set; } = 0;
            public string? Temp { get; set; }
            public int? ownerId { get; set; }
            public int? RepeaterTypeId { get; set; }
            public int PolarityOnLocationId { get; set; }
            public int ItemConnectToId { get; set; }
            public float CenterHigh { get; set; } = 0;
            public float HieghFromLand { get; set; } = 0;
            public float EquivalentSpace { get; set; } = 0;

        }
        public class LibraryAttributesMWDish
        {
            public int MwDishLibraryId { get; set; }

        }
        public class AddCivilLoad
        {
            public DateTime InstallationDate { get; set; } = DateTime.Now;
            public string? ItemOnCivilStatus { get; set; } = null;
            public string? ItemStatus { get; set; } = " ";
            public bool ReservedSpace { get; set; }

        }
    }
}
