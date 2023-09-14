using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.CivilLoadsDTOs;
using TLIS_DAL.ViewModels.DynamicAttInstValueDTOs;

namespace TLIS_DAL.ViewModels.Mw_OtherDTOs
{
    public class AddMw_OtherViewModel
    {
        public string Name { get; set; }
        public string SerialNumber { get; set; }
        public string Notes { get; set; }
        public float HeightBase { get; set; }
        public float HeightLand { get; set; }
        public string VisibleStatus { get; set; }
        public float EquivalentSpace { get; set; }
        public float HBA { get; set; }
        public float Spaceinstallation { get; set; }
        public int mwOtherLibraryId { get; set; }
        public int InstallationPlaceId { get; set; }
        public float HieghFromLand { get; set; }
        public AddCivilLoadsViewModel TLIcivilLoads { get; set; }
        public List<AddDynamicAttInstValueViewModel> TLIdynamicAttInstValue { get; set; }
        public TicketAttributes ticketAtt { get; set; }
    }
}
