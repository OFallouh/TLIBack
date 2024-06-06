using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.ViewModels.LoadOtherDTOs;
using TLIS_DAL.ViewModels.LocationTypeDTOs;
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
            SideArms = new List<LoadandsidearmViewDto>();
            MW_ODUs = new List<LoadandsidearmViewDto>();
            MW_Dishes = new List<LoadandsidearmViewDto>();
            MW_RFUs = new List<LoadandsidearmViewDto>();
            MW_BUs = new List<LoadandsidearmViewDto>();
            MW_Others = new List<LoadandsidearmViewDto>();
            RadioAntennas = new List<LoadandsidearmViewDto>();
            RadioRRUs = new List<LoadandsidearmViewDto>();
            RadioOthers = new List<LoadandsidearmViewDto>();
            Powers = new List<LoadandsidearmViewDto>();
            LoadOthers = new List<LoadandsidearmViewDto>();
        }
        public List<LoadandsidearmViewDto> SideArms { get; set; }
        public List<LoadandsidearmViewDto> MW_ODUs { get; set; }
        public List<LoadandsidearmViewDto> MW_Dishes { get; set; }
        public List<LoadandsidearmViewDto> MW_RFUs { get; set; }
        public List<LoadandsidearmViewDto> MW_BUs { get; set; }
        public List<LoadandsidearmViewDto> MW_Others { get; set; }
        public List<LoadandsidearmViewDto> RadioAntennas { get; set; }
        public List<LoadandsidearmViewDto> RadioRRUs { get; set; }
        public List<LoadandsidearmViewDto> RadioOthers { get; set; }
        public List<LoadandsidearmViewDto> Powers { get; set; }
        public List<LoadandsidearmViewDto> LoadOthers { get; set; }
        public class LoadandsidearmViewDto
        {
            public int  Id { get; set; }
            public string Name { get; set; }
        }
    }
}
