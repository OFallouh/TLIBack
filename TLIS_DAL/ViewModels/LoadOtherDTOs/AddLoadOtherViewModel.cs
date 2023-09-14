using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.ViewModels.CivilLoadsDTOs;
using TLIS_DAL.ViewModels.DynamicAttInstValueDTOs;

namespace TLIS_DAL.ViewModels.LoadOtherDTOs
{
    public class AddLoadOtherViewModel
    {
        public string Name { get; set; }
        public string SerialNumber { get; set; }
        public string Notes { get; set; }
        public float HeightBase { get; set; }
        public float HeightLand { get; set; }
        public float HBA { get; set; }
        public float SpaceInstallation { get; set; }
        public float CenterHigh { get; set; }
        public float HieghFromLand { get; set; }
        public float EquivalentSpace { get; set; }
        public int loadOtherLibraryId { get; set; }
        public int InstallationPlaceId { get; set; }

        public AddCivilLoadsViewModel TLIcivilLoads { get; set; }
        public List<AddDynamicAttInstValueViewModel> TLIdynamicAttInstValue { get; set; }
    }
}
