using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.ViewModels.CabinetDTOs;
using TLIS_DAL.ViewModels.SideArmLibraryDTOs;

namespace TLIS_DAL.ViewModels.SideArmDTOs
{
    public class SideArmLoadDto
    {
        public SideArmViewModel SideArm { get; set; }
        public SideArmLibraryViewModel SideArmLibrary { get; set; }
        public IEnumerable<DynamicAttDto> DynamicAttList { get; set; }
    }
}
