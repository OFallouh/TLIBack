using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModels.LoadOtherLibraryDTOs
{
    public class LoadOtherLibraryViewModel
    {
        public int Id { get; set; }
        public string Model { get; set; }
        public string Note { get; set; }
        public float Length { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public float SpaceLibrary { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }
    }
}
