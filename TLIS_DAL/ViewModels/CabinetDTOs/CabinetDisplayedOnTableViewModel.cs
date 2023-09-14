using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.ViewModels.CabinetPowerLibraryDTOs;
using TLIS_DAL.ViewModels.CabinetTelecomLibraryDTOs;

namespace TLIS_DAL.ViewModels.CabinetDTOs
{
    public class CabinetDisplayedOnTableViewModel
    {
        public CabinetViewModel Cabinet { get; set; }
        public CabinetPowerLibraryViewModel CabinetPowerLibrary { get; set; }
        public CabinetTelecomLibraryViewModel CabinetTelecomLibrary { get; set; }
        public IEnumerable<DynamicAttDto> DynamicAttList { get; set; }
    }
}
