using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;
using TLIS_DAL.ViewModels.LogisticalDTOs;

namespace TLIS_DAL.ViewModels.RadioRRULibraryDTOs
{
    public class EditRadioRRULibraryObject
    {
        public EditRadioRRULibraryAttributes AttributesActivatedLibrary { get; set; }
        public AddLogisticalViewModel LogisticalItems { get; set; }
        public List<AddDdynamicAttributeInstallationValueViewModel> DynamicAttributes { get; set; }
        public class EditRadioRRULibraryAttributes
        {
            public int Id { get; set; }
            public string Model { get; set; }
            public string? Type { get; set; } = "";
            public string? Band { get; set; } = "";
            public float ChannelBandwidth { get; set; }
            public float Weight { get; set; } = 0;
            public string? L_W_H_cm3 { get; set; } = "";
            public float Length { get; set; } = 0;
            public float Width { get; set; } = 0;
            public float Height { get; set; } = 0;
            public string? Notes { get; set; } = "";
            public float Depth { get; set; } = 0;
            public float SpaceLibrary { get; set; } = 0;
            public bool Active { get; set; } 
            public bool Deleted { get; set; } = false;
        }
    }
}

