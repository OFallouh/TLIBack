using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TLIS_DAL.ViewModels.SideArmLibraryDTOs.EditSideArmLibraryObject;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;
using TLIS_DAL.ViewModels.LogisticalDTOs;

namespace TLIS_DAL.ViewModels.GeneratorLibraryDTOs
{
    public class EditGeneratorLibraryObject
    {
        public EditGeneratorLibraryAttributes AttributesActivatedLibrary { get; set; }
        public List<AddDdynamicAttributeInstallationValueViewModel> DynamicAttributes { get; set; }
        public AddLogisticalViewModel LogisticalItems { get; set; }
        public class EditGeneratorLibraryAttributes
        {
            public int Id { get; set; }
            public string Model { get; set; }
            public float Width { get; set; } = 0;
            public float Weight { get; set; } = 0;
            public float Length { get; set; } = 0;
            public string? LayoutCode { get; set; } = " ";
            public float Height { get; set; } = 0;
            public float SpaceLibrary { get; set; } = 0;
            public int? CapacityId { get; set; }
            public bool Active { get; set; } = true;
            public bool Deleted { get; set; } = false;

        }
    }
}
