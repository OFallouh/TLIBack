using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.ViewModels.CabinetDTOs;
using TLIS_DAL.ViewModels.MW_OtherLibraryDTOs;
using TLIS_DAL.ViewModels.RadioAntennaDTOs;
using TLIS_DAL.ViewModels.RadioAntennaLibraryDTOs;

namespace TLIS_DAL.ViewModels.Mw_OtherDTOs
{
    public class MWOtherLoadDto
    {
        public Mw_OtherViewModel MWOther { get; set; }
        public MW_OtherLibraryViewModel MWOtherLibrary { get; set; }
        public IEnumerable<DynamicAttDto> DynamicAttList { get; set; }
    }
}
