using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TLIS_DAL.ViewModels.DynamicAttDTOs;
using TLIS_DAL.ViewModels.LogisticalDTOs;

namespace TLIS_DAL.ViewModels.CabinetTelecomLibraryDTOs
{
    public class AddCabinetTelecomLibraryViewModel
    {
        public string Model { get; set; }
        public float? MaxWeight { get; set; }
        public string LayoutCode { get; set; }
        public string Dimension_W_D_H { get; set; }
        public float Width { get; set; }
        public float Depth { get; set; }
        public float Height { get; set; }
        public float SpaceLibrary { get; set; }
        public int? TelecomTypeId { get; set; } = 0;
        public bool Active { get; set; }
        public bool Deleted { get; set; }
        public AddLogisticalViewModel LogisticalItems { get; set; }
        public List<AddDynamicLibAttValueViewModel> TLIdynamicAttLibValue { get; set; }
        //public List<AddDynamicAttInstViewModel> DynamicAttsInst { get; set; }
    }
}
