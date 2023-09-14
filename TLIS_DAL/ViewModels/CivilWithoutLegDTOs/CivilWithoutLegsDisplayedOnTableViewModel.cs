using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.ViewModels.CabinetDTOs;

namespace TLIS_DAL.ViewModels.CivilWithoutLegDTOs
{
    public class CivilWithoutLegsDisplayedOnTableViewModel
    {
        public CivilWithoutLegViewModel CivilwithoutLeg { get; set; }
        public CivilWithoutLegLibraryViewModel CivilWithoutLegLibrary { get; set; }
        public IEnumerable<DynamicAttDto> DynamicAttList { get; set; }
    }
}
