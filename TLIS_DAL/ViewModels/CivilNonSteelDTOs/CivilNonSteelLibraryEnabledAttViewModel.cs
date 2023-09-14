using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.ViewModels.CabinetDTOs;

namespace TLIS_DAL.ViewModels.CivilNonSteelDTOs
{
    public class CivilNonSteelLibraryEnabledAttViewModel
    {
        public dynamic CivilNonSteelLibraryViewModel { get; set; }
        public IEnumerable<DynamicAttDto> DynamicAttList { get; set; }
    }
}
