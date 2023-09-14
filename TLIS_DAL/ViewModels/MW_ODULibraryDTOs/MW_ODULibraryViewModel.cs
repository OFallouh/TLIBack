using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TLIS_DAL.ViewModels.MW_ODUDTOs
{
    public class MW_ODULibraryViewModel
    {
        public int Id { get; set; }
        public string Model { get; set; }
        public string Note { get; set; }
        public float? Weight { get; set; }
        public string H_W_D { get; set; }
        public float Depth { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public string frequency_range { get; set; }
        public string frequency_band { get; set; }
        public float SpaceLibrary { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }
        public int? parityId { get; set; }
        public string parity_Name { get; set; }
        public float Diameter { get; set; } 
    }
}
