using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.ViewModels.CabinetDTOs;
using TLIS_DAL.ViewModels.LoadOtherLibraryDTOs;
using TLIS_DAL.ViewModels.RadioOtherDTOs;
using TLIS_DAL.ViewModels.RadioOtherLibraryDTOs;

namespace TLIS_DAL.ViewModels.LoadOtherDTOs
{
    public class LoadOtherDto
    {
        public LoadOtherViewModel LoadOther { get; set; }
        public LoadOtherLibraryViewModel LoadOtherLibrary { get; set; }
        public IEnumerable<DynamicAttDto> DynamicAttList { get; set; }
    }
}
