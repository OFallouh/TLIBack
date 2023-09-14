using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.CivilLoadsDTOs;
using TLIS_DAL.ViewModels.DynamicAttInstValueDTOs;

namespace TLIS_DAL.ViewModels.PowerDTOs
{
    public class AddPowerViewModel
    {
        public string Name { get; set; }
        public string SerialNumber { get; set; }
        public float HeightBase { get; set; }
        public float HeightLand { get; set; }
        public float SpaceInstallation { get; set; }
        public float? Height { get; set; }
        public string Notes { get; set; }
        public int? ownerId { get; set; } = 0;
        public int? installationPlaceId { get; set; } = 0;
        public int powerLibraryId { get; set; }
        public float HBA { get; set; }
        public float CenterHigh { get; set; }
        public float HieghFromLand { get; set; }
        public float Azimuth { get; set; }
        public float EquivalentSpace { get; set; }
        public int? powerTypeId { get; set; } = 0;
        public string VisibleStatus { get; set; }
        public AddCivilLoadsViewModel TLIcivilLoads { get; set; }
        public List<AddDynamicAttInstValueViewModel> TLIdynamicAttInstValue { get; set; }
        public TicketAttributes ticketAtt { get; set; }
    }
}
