using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModels.PowerDTOs
{
    public class PowerViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SerialNumber { get; set; }
        public float HeightBase { get; set; }
        public float HeightLand { get; set; }
        public float SpaceInstallation { get; set; }
        public string Notes { get; set; }
        public int? ownerId { get; set; }
        public string owner_Name { get; set; }
        public int? installationPlaceId { get; set; }
        public string installationPlace_Name { get; set; }
        public int powerLibraryId { get; set; }
        public string powerLibrary_Name { get; set; }
        public float EquivalentSpace { get; set; }
        public int? powerTypeId { get; set; }
        public string powerType_Name { get; set; }
        public string VisibleStatus { get; set; }
        public float Azimuth { get; set; }
        public float? Height { get; set; }
    }
}
