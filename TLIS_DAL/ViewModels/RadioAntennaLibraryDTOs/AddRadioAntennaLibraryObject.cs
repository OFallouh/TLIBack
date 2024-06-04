using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;
using TLIS_DAL.ViewModels.LogisticalDTOs;

namespace TLIS_DAL.ViewModels.RadioAntennaLibraryDTOs
{
    public class AddRadioAntennaLibraryObject
    {
        public RadioAntennaLibraryAttributes AttributesActivatedLibrary { get; set; }
        public AddLogisticalViewModel LogisticalItems { get; set; }
        public List<AddDdynamicAttributeInstallationValueViewModel> DynamicAttributes { get; set; }
        public class RadioAntennaLibraryAttributes
        {
            public string Model { get; set; }
            public string? FrequencyBand { get; set; }
            public float Weight { get; set; }
            public float Width { get; set; }
            public float Depth { get; set; }
            public float Length { get; set; }
            public string? Notes { get; set; }
            public float SpaceLibrary { get; set; }
            public bool Active { get; set; } = true;
            public bool Deleted { get; set; } = false;
        }
    }
}
