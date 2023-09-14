using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModels.RadioOtherLibraryDTOs
{
    public class RadioOtherLibraryViewModel
    {
        public int Id { get; set; }
        public string Model { get; set; }
        public float? Weight { get; set; }
        public float Width { get; set; }
        public float Length { get; set; }
        public float Height { get; set; }
        public string Notes { get; set; }
        public float SpaceLibrary { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }
    }
}
