using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;
using TLIS_DAL.ViewModels.LogisticalDTOs;

namespace TLIS_DAL.ViewModels.GeneratorLibraryDTOs
{
    public class AddGeneratorLibraryObject
    {
        public GeneratorLibraryAttributes AttributesActivatedLibrary { get; set; }
        public AddLogisticalViewModel LogisticalItems { get; set; }
        public List<AddDdynamicAttributeInstallationValueViewModel> DynamicAttributes { get; set; }
        public class GeneratorLibraryAttributes
        {
            public string Model { get; set; } 
            public float Width { get; set; } = 0;
            public float Weight { get; set; } = 0;
            public float Length { get; set; } = 0;
            public string? LayoutCode { get; set; } = " ";
            public float Height { get; set; } = 0;
            public float SpaceLibrary { get; set; } = 0;
            public string? GeneratorCapaCity { get; set; } = null;
            public bool Active { get; set; } = true;
            public bool Deleted { get; set; } = false;
            public int? CapacityId { get; set; } = null;


        }
    }
}