using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.CivilNonSteelDTOs;
using TLIS_DAL.ViewModels.CivilWithoutLegDTOs;

namespace TLIS_DAL.ViewModels.CivilWithLegsDTOs
{
    public class AllCivilsViewModel
    {
        public object CivilWithLegs { get; set; }
        public ReturnWithFilters<object> CivilWithoutLeg { get; set; }
        public object CivilNonSteel { get; set; }
    }
}
