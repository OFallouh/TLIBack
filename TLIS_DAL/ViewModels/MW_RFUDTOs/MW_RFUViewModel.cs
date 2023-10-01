using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TLIS_DAL.ViewModels.MW_RFUDTOs
{
    public class MW_RFUViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int MwRFULibraryId { get; set; }
        public string MwRFULibrary_Name { get; set; }
        public int? MwPortId { get; set; }
        public string MwPort_Name { get; set; }
        public float SpaceInstallation { get; set; }
        public float Azimuth { get; set; }
        public float heightBase { get; set; }
        public string? SerialNumber { get; set; }
        public string Note { get; set; }
        public float EquivalentSpace { get; set; }
        public int? OwnerId { get; set; }
        public string Owner_Name { get; set; }
        public float CenterHigh { get; set; }
        public float HBA { get; set; }
        public float HieghFromLand { get; set; }
    }
}
