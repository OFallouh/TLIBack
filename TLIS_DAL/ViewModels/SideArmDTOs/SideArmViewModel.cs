using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TLIS_DAL.ViewModels.SideArmDTOs
{
    public class SideArmViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Notes { get; set; }
        public float? HeightBase { get; set; }
        public float? Azimuth { get; set; }
        public float? ReservedSpace { get; set; }
        public string VisibleStatus { get; set; }
        public float SpaceInstallation { get; set; }
        public int sideArmLibraryId { get; set; }
        public string sideArmLibrary_Name { get; set; }
        public int sideArmInstallationPlaceId { get; set; }
        public string sideArmInstallationPlace_Name { get; set; }
        public int sideArmTypeId { get; set; }
        public string sideArmType_Name { get; set; }
        public int? ownerId { get; set; }
        public string owner_Name { get; set; }
        public bool Active { get; set; }
        public int? ItemStatusId { get; set; }
        public string ItemStatus_Name { get; set; }
        public int? TicketId { get; set; }
        public bool Draft { get; set; }
        public float? HBA { get; set; }
        public float? CenterHigh { get; set; }
        public float? HieghFromLand { get; set; }
    }
}
