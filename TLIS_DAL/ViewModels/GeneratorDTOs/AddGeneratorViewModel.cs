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

namespace TLIS_DAL.ViewModels.GeneratorDTOs
{
    public class AddGeneratorViewModel
    {
        public string Name { get; set; }
        public string SerialNumber { get; set; }
        public int? NumberOfFuelTanks { get; set; }
        public string LocationDescription { get; set; }
        public bool? BaseExisting { get; set; }
        public float SpaceInstallation { get; set; }
        public string VisibleStatus { get; set; }
        public int? BaseGeneratorTypeId { get; set; } = 0;
        public int GeneratorLibraryId { get; set; }
        public AddOtherInSiteViewModel TLIotherInSite { get; set; }
        public AddOtherInventoryDistanceViewModel TLIotherInventoryDistance { get; set; }
        public List<AddDynamicAttInstValueViewModel> TLIdynamicAttInstValue { get; set; }
        public TicketAttributes ticketAtt { get; set; }
    }
}
