using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TLIS_DAL.ViewModels.DynamicAttDTOs;
using TLIS_DAL.ViewModels.LogisticalDTOs;

namespace TLIS_DAL.ViewModels.MW_ODUDTOs
{
    public class AddMW_ODULibraryViewModel
    {
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
        public int? parityId { get; set; } = 0;

        public float Diameter { get; set; } = 0;
        public AddLogisticalViewModel LogisticalItems { get; set; }
        public List<AddDynamicLibAttValueViewModel> TLIdynamicAttLibValue { get; set; }
        //public List<AddDynamicAttInstViewModel> DynamicAttsInst { get; set; }
    }
}
