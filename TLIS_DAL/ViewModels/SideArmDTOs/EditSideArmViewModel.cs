using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.CivilLoadsDTOs;

namespace TLIS_DAL.ViewModels.SideArmDTOs
{
    public class EditSideArmViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Notes { get; set; }
        public float? HeightBase { get; set; }
        public float? HBA { get; set; }
        public float? CenterHigh { get; set; }
        public float? HieghFromLand { get; set; }
        public float? Azimuth { get; set; }
        public float? ReservedSpace { get; set; }
        public string VisibleStatus { get; set; }
        public float SpaceInstallation { get; set; }
        public int sideArmLibraryId { get; set; }
        public int sideArmInstallationPlaceId { get; set; }
        public int? ownerId { get; set; } = 0;
        public int sideArmTypeId { get; set; }
        public int? ItemStatusId { get; set; }
        public int? TicketId { get; set; }
        public bool Draft { get; set; }
        public bool Active { get; set; }
        public EditCivilLoadsViewModel TLIcivilLoads { get; set; }
        public List<BaseInstAttView> DynamicInstAttsValue { get; set; }
    }
}
