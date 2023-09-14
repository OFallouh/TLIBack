using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TLIS_DAL.ViewModels.DynamicAttDTOs;
using TLIS_DAL.ViewModels.LogisticalDTOs;

namespace TLIS_DAL.ViewModels.RadioRRULibraryDTOs
{
    public class AddRadioRRULibraryViewModel
    {
        [Required]
        public string Model { get; set; }
        public string Type { get; set; }
        public string Band { get; set; }
        public float? ChannelBandwidth { get; set; }
        public float? Weight { get; set; }
        public string L_W_H_cm3 { get; set; }
        [Required]
        public float Length { get; set; }
        [Required]
        public float Width { get; set; }
        [Required]
        public float Height { get; set; }
        public string Notes { get; set; }
        [Required]
        public float SpaceLibrary { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }
        public AddLogisticalViewModel LogisticalItems { get; set; }
        public List<AddDynamicLibAttValueViewModel> TLIdynamicAttLibValue { get; set; }
        //public List<AddDynamicAttInstViewModel> DynamicAttsInst { get; set; }
    }
}
