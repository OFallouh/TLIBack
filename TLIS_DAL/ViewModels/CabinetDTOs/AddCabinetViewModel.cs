using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.CivilSiteDateDTOs;
using TLIS_DAL.ViewModels.CivilSupportDistanceDTOs;
using TLIS_DAL.ViewModels.DynamicAttInstValueDTOs;
using TLIS_DAL.ViewModels.OtherInSiteDTOs;
using TLIS_DAL.ViewModels.OtherInventoryDistanceDTOs;

namespace TLIS_DAL.ViewModels.CabinetDTOs
{
    public class AddCabinetViewModel
    {
        public string Name { get; set; }
        public string TPVersion { get; set; }
        public int? RenewableCabinetNumberOfBatteries { get; set; }
        public int? NUmberOfPSU { get; set; }
        public float SpaceInstallation { get; set; }
        public string VisibleStatus { get; set; }
        public int? CabinetPowerLibraryId { get; set; }
        public int? CabinetTelecomLibraryId { get; set; }
        public int? RenewableCabinetTypeId { get; set; } = 0;
        public AddOtherInSiteViewModel TLIotherInSite { get; set; }
        public AddOtherInventoryDistanceViewModel TLIotherInventoryDistance { get; set; }
        public List<AddDynamicAttInstValueViewModel> TLIdynamicAttInstValue { get; set; }
        public TicketAttributes ticketAtt { get; set; }
    }
}
