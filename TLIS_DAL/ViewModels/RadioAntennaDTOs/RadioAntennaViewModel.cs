using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TLIS_DAL.ViewModels.RadioAntennaDTOs
{
    public class RadioAntennaViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Model { get; set; }
        public string Azimuth { get; set; }
        public float? MechanicalTilt { get; set; }
        public float? ElectricalTilt { get; set; }
        public string SerialNumber { get; set; }
        public string HBASurface { get; set; }
        public float EquivalentSpace { get; set; }

        public string Notes { get; set; }
        public float HeightBase { get; set; }
        public float HeightLand { get; set; }
        public float SpaceInstallation { get; set; }
        public int? ownerId { get; set; }
        public string owner_Name { get; set; }
        public int? installationPlaceId { get; set; }
        public string installationPlace_Name { get; set; }
        public int radioAntennaLibraryId { get; set; }
        public string radioAntennaLibrary_Name { get; set; }
    }
}
