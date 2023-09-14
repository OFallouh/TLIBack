using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.GeneratorDTOs;
using TLIS_DAL.ViewModels.SolarDTOs;

namespace TLIS_DAL.ViewModels.CabinetDTOs
{
    public class AllOtherInventoryViewModel
    {
        public ReturnWithFilters<object> Cabinet { get; set; }
        public ReturnWithFilters<object> Solar { get; set; }
        public ReturnWithFilters<object> Generator { get; set; }
    }
}
