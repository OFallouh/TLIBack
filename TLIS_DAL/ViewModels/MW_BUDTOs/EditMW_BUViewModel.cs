using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.CivilLoadsDTOs;

namespace TLIS_DAL.ViewModels.MW_BUDTOs
{
    public class EditMW_BUViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Notes { get; set; }
        public float SpaceInstallation { get; set; }
        public float CenterHigh { get; set; }
        public float HBA { get; set; }
        public float HieghFromLand { get; set; }
        public float EquivalentSpace { get; set; }
        public string Serial_Number { get; set; }
        public float? Height { get; set; }
        public float Azimuth { get; set; }
        public int BUNumber { get; set; }
        public bool Active { get; set; }
        public string Visiable_Status { get; set; }
        public int BaseBUId { get; set; }
        public int? OwnerId { get; set; } = 0;
        public int InstallationPlaceId { get; set; }
        public int? MwBULibraryId { get; set; }
        public int? MainDishId { get; set; } = 0;
        public int? SdDishId { get; set; } = 0;
        public EditCivilLoadsViewModel TLIcivilLoads { get; set; }

        public int PortCascadeId { get; set; }

        public List<BaseInstAttView> DynamicInstAttsValue { get; set; }
    }
}
