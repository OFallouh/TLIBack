using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TLIS_DAL.ViewModels.MW_DishDTOs.AddMWDishInstallationObject;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;

namespace TLIS_DAL.ViewModels.MW_RFUDTOs
{
    public class EditMWRFUInstallationObject
    {
        public InstallationMWDRFUConfigObject installationConfig { get; set; }
        public LibraryAttributesMWDRFUObject civilType { get; set; }
        public AddCivilLoadObject civilLoads { get; set; }
        public installationAttributesMWDRFUObject installationAttributes { get; set; }
        public List<AddDdynamicAttributeInstallationValueViewModel> dynamicAttribute { get; set; }
        public class installationAttributesMWDRFUObject
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public float Azimuth { get; set; }
            public float heightBase { get; set; }
            public string SerialNumber { get; set; }
            public string? Note { get; set; } = null;
            public int? OwnerId { get; set; }
            public float SpaceInstallation { get; set; }
            public float CenterHigh { get; set; }
            public float HBA { get; set; }
            public float HieghFromLand { get; set; }
            public float EquivalentSpace { get; set; }

        }
        public class LibraryAttributesMWDRFUObject
        {
            public int MwRFULibraryId { get; set; }

        }
        public class AddCivilLoadObject
        {
            public DateTime InstallationDate { get; set; } = DateTime.Now;
            public string? ItemOnCivilStatus { get; set; } = null;
            public string? ItemStatus { get; set; } = " ";
            public bool ReservedSpace { get; set; }

        }
        public class InstallationMWDRFUConfigObject
        {
            public int mwBUId { get; set; }
            public string? TX_Frequency { get; set; }
        }
    }
}
