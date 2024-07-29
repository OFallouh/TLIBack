using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;
using TLIS_DAL.ViewModels.LogisticalDTOs;

namespace TLIS_DAL.ViewModels.MW_ODULibraryDTOs
{
    public class ADDMWODULibraryObject
    {
        public MWODULibraryAttributes AttributesActivatedLibrary { get; set; }
        public AddLogisticalViewModel LogisticalItems { get; set; }
        public List<AddDdynamicAttributeInstallationValueViewModel> DynamicAttributes { get; set; }
        public class MWODULibraryAttributes
        {
            public string Model { get; set; }
            public string? Note { get; set; } = " ";
            public float Weight { get; set; } = 0;
            public string? H_W_D { get; set; } = " ";
            public float Depth { get; set; } = 0;
            public float Width { get; set; } = 0;
            public float Height { get; set; } = 0;
            public string? frequency_range { get; set; } = " ";
            public string? frequency_band { get; set; } = " ";
            public float SpaceLibrary { get; set; }
            public bool Active { get; set; } = true;
            public bool Deleted { get; set; } = false;
            public int? parityId { get; set; }
            public float Diameter { get; set; } = 0;

        }
    }
}

