using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.MW_BUDTOs;
using TLIS_DAL.ViewModels.MW_DishDTOs;
using TLIS_DAL.ViewModels.MW_ODUDTOs;
using TLIS_DAL.ViewModels.MW_PortDTOs;
using TLIS_DAL.ViewModels.MW_RFUDTOs;
using TLIS_DAL.ViewModels.PowerDTOs;
using TLIS_DAL.ViewModels.RadioAntennaDTOs;
using TLIS_DAL.ViewModels.RadioRRUDTOs;

namespace TLIS_DAL.ViewModels.LoadPartDTOs
{
    public class LoadsViewModel
    {
        // Microwave...
        public ReturnWithFilters<MW_DishLibInst> MW_Dishes { get; set; }
        public ReturnWithFilters<MW_BULibInst> MW_BUs { get; set; }
        public ReturnWithFilters<MW_ODULibInst> MW_ODUs { get; set; }
        public ReturnWithFilters<MW_RFULibInst> MW_RFUs { get; set; }

        // Radio...
        public ReturnWithFilters<RadioAntennaLibInst> RadioAntennas { get; set; }
        public ReturnWithFilters<RadioRRULibInst> RadioRRUS { get; set; }
        // Power...
        public ReturnWithFilters<PowerLibInst> Powers { get; set; }
    }
}
