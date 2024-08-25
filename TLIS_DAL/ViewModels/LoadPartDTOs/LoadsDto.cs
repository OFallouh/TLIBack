using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.ViewModelBase;
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

namespace TLIS_DAL.ViewModels.LoadPartDTOs
{
    public class LoadsDto
    {
        // Microwave...
        public List<MWDishLoadDto> MW_Dishes { get; set; }
        public  List<MWBULoadDto> MW_BUs { get; set; }
        public List<MWODULoadDto> MW_ODUs { get; set; }
        public List<MWRFULoadDto> MW_RFUs { get; set; }
        public List<MWOtherLoadDto> MW_Others { get; set; }

        // Radio...
        public List<RadioAntennaLoadDto> RadioAntennas { get; set; }
        public List<RadioRRULoadDto> RadioRRUS { get; set; }
        public List<RadioOtherLoadDto> RadioOtherS { get; set; }
        // Power...
        public List<PowerLoadDto>Powers { get; set; }
        //LoadOther
        public List<LoadOtherDto>LoadOthers { get; set; }
        //SideArm
        public List<SideArmLoadDto>SideArms { get; set; }
    }
    public class LoadsDtoInternal
    {
        // Microwave...
        public List<GetForAddMWDishInstallationObject> MWDish { get; set; }
        public List<GetForAddMWDishInstallationObject> MWBU { get; set; }
        public List<GetForAddMWDishInstallationObject> MWODU { get; set; }
        public List<GetForAddMWDishInstallationObject> MWRFU { get; set; }
        public List<GetForAddMWDishInstallationObject> MWOther { get; set; }

        // Radio...
        public List<GetForAddMWDishInstallationObject> RadioAntenna { get; set; }
        public List<GetForAddMWDishInstallationObject> RadioRRU { get; set; }
        public List<GetForAddMWDishInstallationObject> RadioOther { get; set; }
        // Power...
        public List<GetForAddMWDishInstallationObject> Power { get; set; }
        //LoadOther
        public List<GetForAddMWDishInstallationObject> LoadOther { get; set; }
        //SideArm
        public List<GetForAddMWDishInstallationObject> SideArm { get; set; }
    }
}
