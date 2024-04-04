using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.CivilSiteDateDTOs;
using TLIS_DAL.ViewModels.CivilSupportDistanceDTOs;
using TLIS_DAL.ViewModels.DynamicAttInstValueDTOs;

namespace TLIS_DAL.ViewModels.AllCivilInstDTOs
{
    public class AddCivilNonSteel
    {
        public string Name { get; set; }
        public float SpaceInstallation { get; set; }
        public int CivilNonSteelLibraryId { get; set; }
        public int? ownerId { get; set; } = 0;
        public int? supportTypeImplementedId { get; set; } = 0;
        public int? locationTypeId { get; set; } = 0;
        public float locationHeight { get; set; }
        public float BuildingMaxLoad { get; set; }
        public float CivilSupportCurrentLoad { get; set; }
        public float H2Height { get; set; }
        public float HieghFromLand { get; set; }
        public float Support_Limited_Load { get; set; }
        public float CenterHigh { get; set; }
        public float HBA { get; set; }
        public float EquivalentSpace { get; set; }
        public AddCivilSiteDateViewModel TLIcivilSiteDate { get; set; }
        public AddCivilSupportDistanceViewModel CivilSupportDistance { get; set; }
        public List<AddDynamicAttInstValueViewModel> TLIdynamicAttInstValue { get; set; }
        public TicketAttributes ticketAtt { get; set; }
    }
}
