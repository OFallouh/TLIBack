using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TLIS_DAL.ViewModels.DynamicAttDTOs;
using TLIS_DAL.ViewModels.LogisticalDTOs;

namespace TLIS_DAL.ViewModels.CabinetPowerLibraryDTOs
{
    public class EditCabinetPowerLibraryViewModel
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
        public int? CabinetPowerTypeId { get; set; } = 0;
        //public bool Active { get; set; }
        //public bool Deleted { get; set; }
        public List<DynamicAttLibViewModel> DynamicAtts { get; set; }
        public AddLogisticalViewModel LogisticalItems { get; set; }
        //public List<DynaminAttInstViewModel> DynamicAttInst { get; set; }
    }
}
