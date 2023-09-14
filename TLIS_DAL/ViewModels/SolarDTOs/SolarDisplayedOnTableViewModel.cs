using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.ViewModels.CabinetDTOs;
using TLIS_DAL.ViewModels.SolarLibraryDTOs;

namespace TLIS_DAL.ViewModels.SolarDTOs
{
    public class SolarDisplayedOnTableViewModel
    {
        public SolarViewModel Solar { get; set; }
        public SolarLibraryViewModel SolarLibrary { get; set; }
        public IEnumerable<DynamicAttDto> DynamicAttList { get; set; }
    }
}
