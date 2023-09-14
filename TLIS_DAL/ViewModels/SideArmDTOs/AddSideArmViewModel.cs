using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TLIS_DAL.ViewModels.CivilLoadsDTOs;
using TLIS_DAL.ViewModels.DynamicAttInstValueDTOs;
using TLIS_DAL.ViewModels.LegDTOs;

namespace TLIS_DAL.ViewModels.SideArmDTOs
{
    public class AddSideArmViewModel
    {
        public string Name { get; set; }
        public string Notes { get; set; }
        public float? HeightBase { get; set; }
        public float? Azimuth { get; set; }
        public float? HBA { get; set; }
        public float? CenterHigh { get; set; }
        public float? HieghFromLand { get; set; }
        public float? EquivalentSpace { get; set; }
        public float? ReservedSpace { get; set; }
      //  public float? ReservedSpace { get; set; }
        public string VisibleStatus { get; set; }
        public float SpaceInstallation { get; set; }
        public int sideArmLibraryId { get; set; }
        public int sideArmInstallationPlaceId { get; set; }
        public int? ownerId { get; set; } = 0;
        public int sideArmTypeId { get; set; }
      //  public int? ItemStatusId { get; set; } = 0;
        //public int legId { get; set; }
       // public int leg2Id { get; set; }
        public AddCivilLoadsViewModel TLIcivilLoads { get; set; }
      //  public List<LegViewModel> legs { get; set; }
        public List<AddDynamicAttInstValueViewModel> TLIdynamicAttInstValue { get; set; }
    }
}
