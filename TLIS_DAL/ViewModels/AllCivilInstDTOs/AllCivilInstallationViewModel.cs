using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.ViewModelBase;

namespace TLIS_DAL.ViewModels.AllCivilInstDTOs
{
    public class AllCivilInstallationViewModel
    {
        public ReturnWithFilters<object> CivilWithLegs { get; set; }
        public ReturnWithFilters<object> CivilWithoutLegMast { get; set; }
        public ReturnWithFilters<object> CivilWithoutLegCapsule { get; set; }
        public ReturnWithFilters<object> CivilWithoutLegMonopole { get; set; }
        public ReturnWithFilters<object> CivilNonSteel { get; set; }
    }
}
