using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModels.RadioOtherDTOs
{
    public class RadioOtherViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SerialNumber { get; set; }
        public string Notes { get; set; }
        public float HeightBase { get; set; }
        public float HeightLand { get; set; }
        public int ownerId { get; set; }
        public string owner_Name { get; set; }
        public float EquivalentSpace { get; set; }
        public int installationPlaceId { get; set; }
        public string installationPlace_Name { get; set; }
        public int radioOtherLibraryId { get; set; }
        public string radioOtherLibrary_Name { get; set; }
        public float Spaceinstallation { get; set; }
    }
}
