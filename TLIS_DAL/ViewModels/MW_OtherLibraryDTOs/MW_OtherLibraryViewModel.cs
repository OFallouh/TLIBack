using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TLIS_DAL.ViewModels.MW_OtherLibraryDTOs
{
    public class MW_OtherLibraryViewModel
    {
        public int Id { get; set; }
        public string Model { get; set; }
        public string Note { get; set; }
        public float Length { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public string L_W_H { get; set; }
        public string frequency_band { get; set; }
        public float SpaceLibrary { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }
    }
}
