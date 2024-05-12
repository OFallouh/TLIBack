using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;
using TLIS_DAL.ViewModels.LogisticalDTOs;

namespace TLIS_DAL.ViewModels.MW_ODULibraryDTOs
{
    public class EditMWODULibraryObject
    {
        public EditMWODULibraryAttributes AttributesActivatedLibrary { get; set; }
        public List<AddDdynamicAttributeInstallationValueViewModel> DynamicAttributes { get; set; }
        public AddLogisticalViewModel LogisticalItems { get; set; }
        public class EditMWODULibraryAttributes
        {
            public int Id { get; set; }
            public string Model { get; set; }
            public string? Note { get; set; }
            public float Weight { get; set; }
            public string H_W_D { get; set; }
            public float Depth { get; set; }
            public float Width { get; set; }
            public float Height { get; set; }
            public string frequency_range { get; set; }
            public string frequency_band { get; set; }
            public float SpaceLibrary { get; set; }
            public bool Active { get; set; }
            public bool Deleted { get; set; }
            public int? parityId { get; set; }
            public float Diameter { get; set; }

        }
    }
}
