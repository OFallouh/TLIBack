using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TLIS_DAL.ViewModels.RadioRRUDTOs
{
    public class RadioRRUViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SerialNumber { get; set; }
        public string Notes { get; set; }
        public float HeightBase { get; set; }
        public float HeightLand { get; set; }
        public float EquivalentSpace { get; set; }
        [Required]
        public float SpaceInstallation { get; set; }
        public int radioRRULibraryId { get; set; }
        public string radioRRULibrary_Name { get; set; }
        public int? ownerId { get; set; }
        public string owner_Name { get; set; }
        public int? radioAntennaId { get; set; }
        public string radioAntenna_Name { get; set; }
        public int? installationPlaceId { get; set; }
        public string installationPlace_Name { get; set; }
        public bool enable { get; set; }
        public string VisibleStatus { get; set; }
    }
}
