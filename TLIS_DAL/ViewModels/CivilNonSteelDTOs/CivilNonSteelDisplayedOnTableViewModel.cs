using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.ViewModels.CabinetDTOs;
using TLIS_DAL.ViewModels.CivilWithoutLegDTOs;

namespace TLIS_DAL.ViewModels.CivilNonSteelDTOs
{
    public class CivilNonSteelDisplayedOnTableViewModel
    {
        public CivilNonSteelViewModel CivilNonSteel { get; set; }
        public CivilNonSteelLibraryViewModel CivilNonSteelLibrary { get; set; }
        public IEnumerable<DynamicAttDto> DynamicAttList { get; set; }
    }
}
