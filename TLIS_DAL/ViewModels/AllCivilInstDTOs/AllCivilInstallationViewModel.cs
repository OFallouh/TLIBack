using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.CivilWithLegLibraryDTOs;

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
    public class AllCivilInstallation
    {
        public GetEnableAttribute CivilWithLegs { get; set; }
        public GetEnableAttribute CivilWithoutLegMast { get; set; }
        public GetEnableAttribute CivilWithoutLegCapsule { get; set; }
        public GetEnableAttribute CivilWithoutLegMonopole { get; set; }
        public GetEnableAttribute CivilNonSteel { get; set; }
    }
}
