using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TLIS_DAL.ViewModels.RadioAntennaLibraryDTOs
{
    public class RadioAntennaLibraryViewModel
    {
        public int Id { get; set; }
        public string Model { get; set; }
        public string FrequencyBand { get; set; }
        public float? Weight { get; set; }
        public float Width { get; set; }
        public float Depth { get; set; }
        public float Length { get; set; }
        public string Notes { get; set; }
        public float SpaceLibrary { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }
    }
}
