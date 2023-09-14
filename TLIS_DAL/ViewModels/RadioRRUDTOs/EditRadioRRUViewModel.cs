using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.CivilLoadsDTOs;
using TLIS_DAL.ViewModels.DynamicAttInstValueDTOs;

namespace TLIS_DAL.ViewModels.RadioRRUDTOs
{
    public class EditRadioRRUViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SerialNumber { get; set; }
        public string Notes { get; set; }
        [Required]
        public float HeightBase { get; set; }
        [Required]
        public float HeightLand { get; set; }
        public float CenterHigh { get; set; }
        public float HBA { get; set; }
        public float HieghFromLand { get; set; }
        public float EquivalentSpace { get; set; }
        public float SpaceInstallation { get; set; }
        public string VisibleStatus { get; set; }
        public int? radioRRULibraryId { get; set; }
        public int? ownerId { get; set; } = 0;
        public int? radioAntennaId { get; set; } = 0;
        public int? installationPlaceId { get; set; } = 0;
        public float Azimuth { get; set; }
        //  public List<DynamicAttInstValueViewModel> DynamicAttInstValues { get; set; }
        public EditCivilLoadsViewModel TLIcivilLoads { get; set; }
        public List<BaseInstAttView> DynamicInstAttsValue { get; set; }
    }
}
