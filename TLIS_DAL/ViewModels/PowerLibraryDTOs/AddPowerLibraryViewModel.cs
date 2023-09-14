using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TLIS_DAL.ViewModels.DynamicAttDTOs;
using TLIS_DAL.ViewModels.LogisticalDTOs;

namespace TLIS_DAL.ViewModels.PowerDTOs
{
    public class AddPowerLibraryViewModel
    {
        public string Model { get; set; }
        public string Note { get; set; }
        public float Weight { get; set; }
        public string FrequencyRange { get; set; }
        public string Type { get; set; }
        public float Size { get; set; }
        public string L_W_H { get; set; }
        public float width { get; set; }
        public float Length { get; set; }
        public float Depth { get; set; }
        public float SpaceLibrary { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }
        public AddLogisticalViewModel LogisticalItems { get; set; }
        public List<AddDynamicLibAttValueViewModel> TLIdynamicAttLibValue { get; set; }
        //public List<AddDynamicAttInstViewModel> DynamicAttsInst { get; set; }
    }
}
