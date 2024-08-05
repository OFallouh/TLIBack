using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TLIS_DAL.ViewModels.MW_DishDTOs.AddMWDishInstallationObject;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;
using TLIS_DAL.Models;

namespace TLIS_DAL.ViewModels.MW_RFUDTOs
{
    public class AddMWRFUInstallation
    {
        public InstallationAttributesMWRFUConfigObject installationConfig { get; set; }
        public AddCivilLoad civilLoads { get; set; }
        public InstallationMWRFUConfigObject installationAttributes { get; set; }
        public List<AddDdynamicAttributeInstallationValueViewModel> dynamicAttribute { get; set; }
        public class InstallationMWRFUConfigObject
        {
            public string Name { get; set; }
            public float Azimuth { get; set; }
            public float heightBase { get; set; }
            public string SerialNumber { get; set; }
            public string? Note { get; set; }
            public int? OwnerId { get; set; }
            public float SpaceInstallation { get; set; }
            public float CenterHigh { get; set; }
            public float HBA { get; set; }
            public float HieghFromLand { get; set; }
            public float EquivalentSpace { get; set; }
        }
        public class InstallationAttributesMWRFUConfigObject
        {
            public int MwRFULibraryId { get; set; }
            public int mwBUId { get; set; }
            public string? TX_Frequency { get; set; }
        }

    }
}