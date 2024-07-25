using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;
using TLIS_DAL.ViewModels.LogisticalDTOs;

namespace TLIS_DAL.ViewModels.RadioOtherLibraryDTOs
{
    public class EditRadioOtherLibraryObject
    {
        public EditRadioOtherLibraryAttributes AttributesActivatedLibrary { get; set; }
        public AddLogisticalViewModel LogisticalItems { get; set; }
        public List<AddDdynamicAttributeInstallationValueViewModel> DynamicAttributes { get; set; }
        public class EditRadioOtherLibraryAttributes
        {
            public int Id { get; set; }
            public string Model { get; set; }
            public float Weight { get; set; } = 0;
            public float Width { get; set; } = 0;
            public float Length { get; set; } = 0;
            public float Height { get; set; } = 0;
            public string? Notes { get; set; } = " ";
            public float SpaceLibrary { get; set; } = 0;
            public bool Active { get; set; } = true;
            public bool Deleted { get; set; } = false;
        }
    }
}