using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.CivilLoadsDTOs;
using TLIS_DAL.ViewModels.DynamicAttInstValueDTOs;

namespace TLIS_DAL.ViewModels.MW_RFUDTOs
{
    public class AddMW_RFUViewModel
    {
        public string Name { get; set; }
        public int MwRFULibraryId { get; set; }
        public double Azimuth { get; set; }
        public float heightBase { get; set; }
        public int? OwnerId { get; set; } = 0;
        public string Note { get; set; }
        public string? SerialNumber { get; set; }
        public float EquivalentSpace { get; set; }
        public float HBA { get; set; }
        public float CenterHigh { get; set; }

        public float HieghFromLand { get; set; }
        public int? MwPortId { get; set; } = 0;
        public float SpaceInstallation { get; set; }
        public AddCivilLoadsViewModel TLIcivilLoads { get; set; }
        public List<AddDynamicAttInstValueViewModel> TLIdynamicAttInstValue { get; set; }
        public TicketAttributes ticketAtt { get; set; }
    }
}
