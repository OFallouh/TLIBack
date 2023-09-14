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
using TLIS_DAL.ViewModels.SideArmDTOs;

namespace TLIS_DAL.ViewModels.CivilLoadsDTOs
{
    public class CivilLoads
    {
        public CivilLoads()
        {
            SideArms = new List<SideArmViewModel>();
            MW_ODUs = new List<MW_ODUViewModel>();
            MW_Dishes = new List<MW_DishViewModel>();
            MW_RFUs = new List<MW_RFUViewModel>();
            MW_BUs = new List<MW_BUViewModel>();
            MW_Others = new List<Mw_OtherViewModel>();
            RadioAntennas = new List<RadioAntennaViewModel>();
            RadioRRUs = new List<RadioRRUViewModel>();
            RadioOthers = new List<RadioOtherViewModel>();
            Powers = new List<PowerViewModel>();
            LoadOthers = new List<LoadOtherViewModel>();
        }
        public List<SideArmViewModel> SideArms { get; set; }
        public List<MW_ODUViewModel> MW_ODUs { get; set; }
        public List<MW_DishViewModel> MW_Dishes { get; set; }
        public List<MW_RFUViewModel> MW_RFUs { get; set; }
        public List<MW_BUViewModel> MW_BUs { get; set; }
        public List<Mw_OtherViewModel> MW_Others { get; set; }
        public List<RadioAntennaViewModel> RadioAntennas { get; set; }
        public List<RadioRRUViewModel> RadioRRUs { get; set; }
        public List<RadioOtherViewModel> RadioOthers { get; set; }
        public List<PowerViewModel> Powers { get; set; }
        public List<LoadOtherViewModel> LoadOthers { get; set; }
    }
}
