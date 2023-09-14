using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.ViewModels.CabinetDTOs;

namespace TLIS_DAL.ViewModels.CivilWithLegsDTOs
{
    public class CivilWithLegsLibraryEnabledAttViewModel
    {
        public dynamic CivilWithLegsLibraryViewModel { get; set; }
        public IEnumerable<DynamicAttDto> DynamicAttList { get; set; }
    }
}
