using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TLIS_DAL.ViewModels.CabinetPowerLibraryDTOs
{
    public class CabinetPowerLibraryViewModel
    {
        public int Id { get; set; }
        public string Model { get; set; }
        public float? Weight { get; set; }
        public int? NumberOfBatteries { get; set; }
        public string LayoutCode { get; set; }
        public string Dimension_W_D_H { get; set; }
        public float? BatteryWeight { get; set; }
        public string BatteryType { get; set; }
        public string BatteryDimension_W_D_H { get; set; }
        public float Depth { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public float SpaceLibrary { get; set; }
        public int? CabinetPowerTypeId { get; set; }
        public string CabinetPowerType_Name { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }
    }
}
