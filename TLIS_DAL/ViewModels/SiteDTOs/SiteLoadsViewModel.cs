using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.ViewModels.LoadOtherDTOs;
using TLIS_DAL.ViewModels.MW_BUDTOs;
using TLIS_DAL.ViewModels.MW_DishDTOs;
using TLIS_DAL.ViewModels.MW_ODUDTOs;
using TLIS_DAL.ViewModels.Mw_OtherDTOs;
using TLIS_DAL.ViewModels.MW_RFUDTOs;
using TLIS_DAL.ViewModels.PowerDTOs;
using TLIS_DAL.ViewModels.RadioAntennaDTOs;
using TLIS_DAL.ViewModels.RadioOtherDTOs;
using TLIS_DAL.ViewModels.RadioRRUDTOs;

namespace TLIS_DAL.ViewModels.SiteDTOs
{
    public class SiteLoadsViewModel
    {
        public List<MW_BUViewModel> MW_BUs { get; set; }
        public List<MW_DishViewModel> MW_Dishes { get; set; }
        public List<MW_ODUViewModel> MW_ODUs { get; set; }
        public List<MW_RFUViewModel> MW_RFUs { get; set; }
        public List<Mw_OtherViewModel> MW_Others { get; set; }
        public List<RadioAntennaViewModel> radioAntennas { get; set; }
        public List<RadioRRUViewModel> radioRRUs { get; set; }
        public List<RadioOtherViewModel> radioOthers { get; set; }
        public List<PowerViewModel> powers { get; set; }
        public List<LoadOtherViewModel> loadOthers { get; set; }
    }
}
