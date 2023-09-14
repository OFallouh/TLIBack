using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.ViewModels.CabinetDTOs;

namespace TLIS_DAL.ViewModels.LoadOtherLibraryDTOs
{
    public class LoadOtherLibrariesDisplayedOnTableViewModel
    {
        public object LoadOtehrLibrary { get; set; }
        public IEnumerable<DynamicAttDto> DynamicAttList { get; set; }
    }
}
