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
        public InstallationConfigObject installationConfig { get; set; }
        public installationAttributesSideArms installationAttributes { get; set; }
        public AddCivilLoads civilLoads { get; set; }
        public List<AddDdynamicAttributeInstallationValueViewModel> dynamicAttribute { get; set; }
        public class InstallationConfigObject
        {
            public int installationPlaceId { get; set; }
            public int? civilSteelType { get; set; }
            public int? civilWithLegId { get; set; }
            public int? civilWithoutLegId { get; set; }
            public int? civilNonSteelId { get; set; }
            public int sideArmTypeId { get; set; }
            public List<int>? legId { get; set; }
            public int? branchingSideArmId { get; set; }
            public int sideArmLibraryId { get; set; }
        }
        public class installationAttributesSideArms
        {
            public string Name { get; set; }
            public string? Notes { get; set; } = " ";
            public float HeightBase { get; set; } = 0;
            public float Azimuth { get; set; } = 0;
            public float ReservedSpace { get; set; } = 0;
            public bool Active { get; set; } = true;
            public string? VisibleStatus { get; set; } = " ";
            public float SpaceInstallation { get; set; } = 0;
            public int? ownerId { get; set; } = null;
            public int? ItemStatusId { get; set; } = null;
            public int? TicketId { get; set; } = null;
            public bool Draft { get; set; } = false;
            public float CenterHigh { get; set; } = 0;
            public float HBA { get; set; } = 0;
            public float HieghFromLand { get; set; } = 0;
            public float EquivalentSpace { get; set; } = 0;
        }
        public class AddCivilLoads
        {
            public DateTime InstallationDate { get; set; } = DateTime.Now;
            public string? ItemOnCivilStatus { get; set; } = null;
            public string? ItemStatus { get; set; } = " ";
            public bool Dismantle { get; set; }
            public bool ReservedSpace { get; set; } = false;
            public int? BranchingSideArmId { get; set; } = null;
            public int? civilSteelSupportCategoryId { get; set; } = null;

        }
    }
}
