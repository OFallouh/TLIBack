using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.CivilLoadsDTOs;

namespace TLIS_DAL.ViewModels.MW_RFUDTOs
{
    public class EditMW_RFUViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Note { get; set; }
        public float Azimuth { get; set; }
        public float SpaceInstallation { get; set; }

        public float CenterHigh { get; set; }
        public float HBA { get; set; }
        public float HieghFromLand { get; set; }
        public float EquivalentSpace { get; set; }
        public int? MwRFULibraryId { get; set; }
        public float heightBase { get; set; }
        public string? SerialNumber { get; set; }
        public int? MwPortId { get; set; } = 0;
        public int? OwnerId { get; set; } = 0;
        public EditCivilLoadsViewModel TLIcivilLoads { get; set; }
        public List<BaseInstAttView> DynamicInstAttsValue { get; set; }
    }
}
