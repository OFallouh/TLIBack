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
        public CivilNonSteelLibraryAttributes LibraryAttribute { get; set; }
        public AddLogisticalViewModel LogisticalItems { get; set; }
        public List<AddDdynamicAttributeInstallationValueViewModel> dynamicAttribute { get; set; }
        public class CivilNonSteelLibraryAttributes
        {
            public string Note { get; set; }
            public float Hight { get; set; } = 0;
            public float SpaceLibrary { get; set; } = 0;
            public string Prefix { get; set; }
            public bool VerticalMeasured { get; set; }=false;
            public int civilNonSteelTypeId { get; set; } 
            public float NumberofBoltHoles { get; set; } = 0;
            public bool Active { get; set; } = true;
            public bool Deleted { get; set; } = false;
            public float Manufactured_Max_Load { get; set; } = 0;
        }
    }
}
