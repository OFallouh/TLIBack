using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;
using TLIS_DAL.ViewModels.DynamicAttDTOs;
using TLIS_DAL.ViewModels.DynamicAttInstValueDTOs;
using TLIS_DAL.ViewModels.LogisticalDTOs;
using TLIS_DAL.ViewModels.LogisticalitemDTOs;
using static TLIS_DAL.ViewModels.CivilWithoutLegDTOs.AddCivilWithoutLegLibraryViewModel;

namespace TLIS_DAL.ViewModels.CivilNonSteelDTOs
{
   public class AddCivilNonSteelLibraryViewModel
    {
        public string Model { get; set; }
        public string Note { get; set; }
        public float Hight { get; set; }
        public float SpaceLibrary { get; set; }
        public bool VerticalMeasured { get; set; }
        public int civilNonSteelTypeId { get; set; } = 0;
        public float NumberofBoltHoles { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }
        public float Manufactured_Max_Load { get; set; }
        public AddLogisticalViewModel LogisticalItems { get; set; }
        public List<AddDynamicLibAttValueViewModel> TLIdynamicAttLibValue { get; set; }
    }
}
