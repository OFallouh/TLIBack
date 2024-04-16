using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.CivilSiteDateDTOs;
using TLIS_DAL.ViewModels.CivilSupportDistanceDTOs;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;
using TLIS_DAL.ViewModels.LegDTOs;

namespace TLIS_DAL.ViewModels
{
    public class AddCivilNonSteelObject
    {
        public LibraryAttributesCivilNonSteel civilType { get; set; }
        public installationAttributesCivilNonSteel installationAttributes { get; set; }
        public AddCivilSiteDateViewModel civilSiteDate { get; set; }
        public AddCivilSupportDistanceViewModel civilSupportDistance { get; set; }
        public List<AddDdynamicAttributeInstallationValueViewModel> dynamicAttribute { get; set; }
        public class LibraryAttributesCivilNonSteel
        {
            public int CivilNonSteelLibraryId { get; set; }
        }
        public class installationAttributesCivilNonSteel
        {
            public string Name { get; set; }
            public double CurrentLoads { get; set; } = 0;
            public float SpaceInstallation { get; set; } = 0;
            public int? ownerId { get; set; }
            public int? supportTypeImplementedId { get; set; }
            public int? locationTypeId { get; set; }
            public float locationHeight { get; set; } = 0;
            public float BuildingMaxLoad { get; set; } = 0;
            public float CivilSupportCurrentLoad { get; set; } = 0;
            public float H2Height { get; set; } = 0;
            public float Support_Limited_Load { get; set; } = 0;
            public float CenterHigh { get; set; } = 0;
            public float HBA { get; set; } = 0;
            public float HieghFromLand { get; set; } = 0;
            public float EquivalentSpace { get; set; } = 0;
        }
    }
}
