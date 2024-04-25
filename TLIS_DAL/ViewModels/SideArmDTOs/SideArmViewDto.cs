using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.ViewModels.CivilLoadsDTOs;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;
using TLIS_DAL.ViewModels.LogisticalDTOs;

namespace TLIS_DAL.ViewModels.SideArmDTOs
{
    public class SideArmViewDto
    {
        public LibraryAttributesSideArms civilType { get; set; }
        public installationAttributesSideArms installationAttributes { get; set; }
        public AddCivilLoadsViewModel TLIcivilLoads { get; set; }
        public List<AddDdynamicAttributeInstallationValueViewModel> dynamicAttributes { get; set; }
        public class LibraryAttributesSideArms
        {
            public int sideArmLibraryId { get; set; }
        }
        public class installationAttributesSideArms
        {
            public string Name { get; set; }
            public string? Notes { get; set; }
            public float HeightBase { get; set; }
            public float Azimuth { get; set; }
            public float ReservedSpace { get; set; }
            public bool Active { get; set; }
            public string? VisibleStatus { get; set; }
            public float SpaceInstallation { get; set; }
            public int sideArmInstallationPlaceId { get; set; }
            public int? ownerId { get; set; }
            public int sideArmTypeId { get; set; }
            public int? ItemStatusId { get; set; }
            public int? TicketId { get; set; }
            public bool Draft { get; set; }
            public float CenterHigh { get; set; }
            public float HBA { get; set; }
            public float HieghFromLand { get; set; }
            public float EquivalentSpace { get; set; }
        }
    }
}
