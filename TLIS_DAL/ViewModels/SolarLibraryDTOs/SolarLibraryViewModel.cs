using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModels.SolarLibraryDTOs
{
    public class SolarLibraryViewModel
    {
        public int Id { get; set; }
        public string Model { get; set; }
        public float? Weight { get; set; }
        public string TotaPanelsDimensions { get; set; }
        public string StructureDesign { get; set; }
        public string LayoutCode { get; set; }
        public float? HeightFromFront { get; set; }
        public float? HeightFromBack { get; set; }
        public string BasePlateDimension { get; set; }
        public float SpaceLibrary { get; set; }
        public int? CapacityId { get; set; }
        public string Capacity_Name { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }
    }
}
