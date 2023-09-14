using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TLIS_DAL.ViewModels.PowerDTOs
{
   public class PowerLibraryViewModel
    {
        public int Id { get; set; }
        public string Model { get; set; }
        public string Note { get; set; }
        public float Weight { get; set; }
        public float width { get; set; }
        public float Length { get; set; }
        public float Depth { get; set; }
        public float SpaceLibrary { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }
        public string FrequencyRange { get; set; }
        public string Type { get; set; }
        public string L_W_H { get; set; }
        public float Size { get; set; }
    }
}
