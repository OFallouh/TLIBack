using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TLIS_DAL.ViewModels.DynamicAttDTOs;
using TLIS_DAL.ViewModels.LogisticalDTOs;

namespace TLIS_DAL.ViewModels.RadioAntennaLibraryDTOs
{
    public class AddRadioAntennaLibraryViewModel
    {
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
        public AddLogisticalViewModel LogisticalItems { get; set; }
        public List<AddDynamicLibAttValueViewModel> TLIdynamicAttLibValue { get; set; }
        //public List<AddDynamicAttInstViewModel> DynamicAttsInst { get; set; }
    }
}
