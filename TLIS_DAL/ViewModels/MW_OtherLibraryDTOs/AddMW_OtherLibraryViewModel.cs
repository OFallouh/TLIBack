using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TLIS_DAL.ViewModels.DynamicAttDTOs;
using TLIS_DAL.ViewModels.LogisticalDTOs;

namespace TLIS_DAL.ViewModels.MW_OtherLibraryDTOs
{
    public class AddMW_OtherLibraryViewModel
    {
        public string Model { get; set; }
        public string Note { get; set; }
        public float Length { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public string L_W_H { get; set; }
        public string frequency_band { get; set; }
        public float SpaceLibrary { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }
        public AddLogisticalViewModel LogisticalItems { get; set; }
        public List<AddDynamicLibAttValueViewModel> TLIdynamicAttLibValue { get; set; }
    }
}
