using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.ViewModels.CivilLoadsDTOs;
using TLIS_DAL.ViewModels.DynamicAttInstValueDTOs;

namespace TLIS_DAL.ViewModels.RadioOtherDTOs
{
    public class AddRadioOtherIntegration
    {
        public string Name { get; set; }
        public string SerialNumber { get; set; }
        public string Notes { get; set; }
        public float HeightBase { get; set; }
        public float HeightLand { get; set; }
        public float HBA { get; set; }
        public int ownerId { get; set; } = 0;
        public int? installationPlaceId { get; set; } = 0;
        public int radioOtherLibraryId { get; set; }
        public float CenterHigh { get; set; }
        public float HieghFromLand { get; set; }
        public float EquivalentSpace { get; set; }
        public float Spaceinstallation { get; set; }
        public AddCivilLoadsViewModel TLIcivilLoads { get; set; }
        public List<AddDynamicAttInstValueViewModel> TLIdynamicAttInstValue { get; set; }
    }
}
