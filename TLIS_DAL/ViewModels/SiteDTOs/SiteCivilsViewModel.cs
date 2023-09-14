using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.ViewModels.CivilNonSteelDTOs;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;
using TLIS_DAL.ViewModels.CivilWithoutLegDTOs;

namespace TLIS_DAL.ViewModels.SiteDTOs
{
    public class SiteCivilsViewModel
    {
        public List<CivilWithLegsViewModel> CivilWithLegs { get; set; }
        public List<CivilWithoutLegViewModel> civilWithoutLegs { get; set; }
        public List<CivilNonSteelViewModel> civilNonSteels { get; set; }
    }
}
