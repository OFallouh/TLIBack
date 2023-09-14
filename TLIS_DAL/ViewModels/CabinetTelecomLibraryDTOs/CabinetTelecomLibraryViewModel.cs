using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModels.CabinetTelecomLibraryDTOs
{
    public class CabinetTelecomLibraryViewModel
    {
        public int Id { get; set; }
        public string Model { get; set; }
        public float? MaxWeight { get; set; }
        public string LayoutCode { get; set; }
        public string Dimension_W_D_H { get; set; }
        public float Width { get; set; }
        public float Depth { get; set; }
        public float Height { get; set; }
        public float SpaceLibrary { get; set; }
        public int? TelecomTypeId { get; set; }
        public string TelecomType_Name { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }
    }
}
