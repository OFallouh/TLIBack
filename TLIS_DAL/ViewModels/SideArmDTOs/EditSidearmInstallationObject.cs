using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.ViewModels.CivilLoadsDTOs;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;
using static TLIS_DAL.ViewModels.SideArmDTOs.SideArmViewDto;

namespace TLIS_DAL.ViewModels.SideArmDTOs
{
    public class EditSidearmInstallationObject
    {
        public InstallationConfigObject installationConfig { get; set; }
        public LibraryAttributesSideArm civilType { get; set; }
        public EditinstallationAttributesSideArm installationAttributes { get; set; }
        public EditCivilLoad CivilLoads { get; set; }
        public List<AddDdynamicAttributeInstallationValueViewModel> dynamicAttribute { get; set; }
        public class LibraryAttributesSideArm
        {
            public int sideArmLibraryId { get; set; }
        }
        public class EditinstallationAttributesSideArm
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string? Notes { get; set; }=" ";
            public float HeightBase { get; set; } = 0;
            public float Azimuth { get; set; } = 0;
            public bool ReservedSpace { get; set; } = false;
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
        public class EditCivilLoad
        {
            public DateTime InstallationDate { get; set; }=DateTime.Now;
            public string? ItemOnCivilStatus { get; set; } = " ";
            public string? ItemStatus { get; set; } = " ";
            public bool ReservedSpace { get; set; }
            public bool Dismantle { get; set; } = false;
        }
    }
}
 