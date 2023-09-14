using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.CivilLoadsDTOs;

namespace TLIS_DAL.ViewModels.MW_DishDTOs
{
    public class EditMW_DishViewModel
    {
        public int Id { get; set; }
        public string DishName { get; set; }
        public float Azimuth { get; set; }
        public string Notes { get; set; }
        public string Far_End_Site_Code { get; set; }
        public string HBA_Surface { get; set; }
        public bool Is_Repeator { get; set; }
        public string Serial_Number { get; set; }
        public float SpaceInstallation { get; set; }
        public float CenterHigh { get; set; }
        public float HBA { get; set; }
        public float HieghFromLand { get; set; }
        public float EquivalentSpace { get; set; }
        public string MW_LinkId { get; set; }
        public string Visiable_Status { get; set; }
        public float HeightBase { get; set; }
        public float HeightLand { get; set; }
        public int? RepeaterTypeId { get; set; } = 0;
        public int? PolarityOnLocationId { get; set; } = 0;
        public int? ItemConnectToId { get; set; } = 0;
        public int? MwDishLibraryId { get; set; }
        public string Temp { get; set; }
        public int? InstallationPlaceId { get; set; } = 0;
        public int? ownerId { get; set; } = 0;
        public EditCivilLoadsViewModel TLIcivilLoads { get; set; }
        public List<BaseInstAttView> DynamicInstAttsValue { get; set; }
    }
}
