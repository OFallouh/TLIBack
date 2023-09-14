using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TLIS_DAL.ViewModels.GeneratorLibraryDTOs
{
    public class GeneratorLibraryViewModel
    {
        public int Id { get; set; }
        public string Model { get; set; }
        public float Width { get; set; }
        public float? Weight { get; set; }
        public float Length { get; set; }
        public string LayoutCode { get; set; }
        public float Height { get; set; }
        public float SpaceLibrary { get; set; }
        public int? CapacityId { get; set; }
        public string Capacity_Name { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }
    }
}
