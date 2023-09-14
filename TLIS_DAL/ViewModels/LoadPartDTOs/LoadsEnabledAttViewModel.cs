using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.MW_BUDTOs;
using TLIS_DAL.ViewModels.MW_DishDTOs;
using TLIS_DAL.ViewModels.MW_ODUDTOs;
using TLIS_DAL.ViewModels.MW_RFUDTOs;
using TLIS_DAL.ViewModels.PowerDTOs;
using TLIS_DAL.ViewModels.RadioAntennaDTOs;
using TLIS_DAL.ViewModels.RadioRRUDTOs;

namespace TLIS_DAL.ViewModels.LoadPartDTOs
{
    public class LoadsEnabledAttViewModel
    {
        // Microwave...
        public ReturnWithFilters<MW_DishEnabledAttViewModel> MW_Dishes { get; set; }
        public ReturnWithFilters<MW_BUEnabledAttViewModel> MW_BUs { get; set; }
        public ReturnWithFilters<MW_ODUEnabledAttViewModel> MW_ODUs { get; set; }
        public ReturnWithFilters<MW_RFUEnabledAttViewModel> MW_RFUs { get; set; }

        // Radio...
        public ReturnWithFilters<RadioAntennaEnabledAttViewModel> RadioAntennas { get; set; }
        public ReturnWithFilters<RadioRRUEnabledAttViewModel> RadioRRUS { get; set; }
        // Power...
        public ReturnWithFilters<PowerEnabledAttViewModel> Powers { get; set; }
    }
}
