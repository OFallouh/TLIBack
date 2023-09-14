using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.CabinetDTOs;
using TLIS_DAL.ViewModels.CivilWithLegDTOs;
using TLIS_DAL.ViewModels.DynamicAttDTOs;

namespace TLIS_DAL.ViewModels.CivilWithLegsDTOs
{
    public class CivilWithLegsDisplayedOnTableViewModel
    {
        public CivilWithLegsViewModel Civilwithlegs { get; set; }
        public CivilWithLegLibraryViewModel CivilWithLegLibrary { get; set; }
        public IEnumerable<DynamicAttDto> DynamicAttList { get; set; }
    }
}
