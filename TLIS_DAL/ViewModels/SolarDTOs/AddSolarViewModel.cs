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

namespace TLIS_DAL.ViewModels.SolarDTOs
{
    public class AddSolarViewModel
    {
        public string Name { get; set; }
        public string PVPanelBrandAndWattage { get; set; }
        public string PVArrayAzimuth { get; set; }
        public string PVArrayAngel { get; set; }
        public string Prefix { get; set; }
        public string PowerLossRatio { get; set; }
        public int NumberOfSSU { get; set; }
        public int NumberOfLightingRod { get; set; }
        public int NumberOfInstallPVs { get; set; }
        public string LocationDescription { get; set; }
        public string ExtenstionDimension { get; set; }
        public string Extension { get; set; }
        public float SpaceInstallation { get; set; }
        public string VisibleStatus { get; set; }
        public int SolarLibraryId { get; set; }
        public int? CabinetId { get; set; } = 0;
        public AddOtherInSiteViewModel TLIotherInSite { get; set; }
        public AddOtherInventoryDistanceViewModel TLIotherInventoryDistance { get; set; }
        public List<AddDynamicAttInstValueViewModel> TLIdynamicAttInstValue { get; set; }
        public TicketAttributes ticketAtt { get; set; }
    }
}
