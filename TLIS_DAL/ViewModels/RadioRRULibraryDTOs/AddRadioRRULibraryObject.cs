using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;
using TLIS_DAL.ViewModels.LogisticalDTOs;
using static TLIS_DAL.ViewModels.RadioAntennaLibraryDTOs.AddRadioAntennaLibraryObject;

namespace TLIS_DAL.ViewModels.RadioRRULibraryDTOs
{
    public class AddRadioRRULibraryObject
    {
        public RadioRRULibraryAttributes AttributesActivatedLibrary { get; set; }
        public AddLogisticalViewModel LogisticalItems { get; set; }
        public List<AddDdynamicAttributeInstallationValueViewModel> DynamicAttributes { get; set; }
        public class RadioRRULibraryAttributes
        {
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
            public bool Active { get; set; } = true;
            public bool Deleted { get; set; } = false;

        }
    }
}
