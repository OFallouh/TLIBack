using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;
using TLIS_DAL.ViewModels.LogisticalDTOs;

namespace TLIS_DAL.ViewModels.SolarLibraryDTOs
{

    public class AddSolarLibraryObject
    {
        public SolarLibraryAttributes AttributesActivatedLibrary { get; set; }
        public AddLogisticalViewModelSolar LogisticalItems { get; set; }
        public List<AddDdynamicAttributeInstallationValueViewModel> DynamicAttributes { get; set; }
        public class SolarLibraryAttributes
        {
            public string Model { get; set; }
            public float Weight { get; set; } = 0;
            public float Length { get; set; } = 0;
            public float Width { get; set; } = 0;
            public string? TotaPanelsDimensions { get; set; } = " ";
            public string? StructureDesign { get; set; } = " ";
            public string? LayoutCode { get; set; } = " ";
            public float HeightFromFront { get; set; } = 0;
            public float HeightFromBack { get; set; } = 0;
            public string? BasePlateDimension { get; set; } = " ";
            public float SpaceLibrary { get; set; } = 0;
            public bool Active { get; set; } = true;
            public bool Deleted { get; set; } = false;
            public int? CapacityId { get; set; } = null;


        }
    }
}