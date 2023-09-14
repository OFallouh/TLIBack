using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.ViewModels.CabinetDTOs;
using TLIS_DAL.ViewModels.Mw_OtherDTOs;
using TLIS_DAL.ViewModels.MW_OtherLibraryDTOs;
using TLIS_DAL.ViewModels.RadioOtherLibraryDTOs;

namespace TLIS_DAL.ViewModels.RadioOtherDTOs
{
    public class RadioOtherLoadDto
    {
        public RadioOtherViewModel RadioOther { get; set; }
        public RadioOtherLibraryViewModel RadioOtherLibrary { get; set; }
        public IEnumerable<DynamicAttDto> DynamicAttList { get; set; }
    }
}
