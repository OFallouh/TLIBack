using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;
using TLIS_DAL.ViewModels.LogisticalDTOs;

namespace TLIS_DAL.ViewModels.PowerLibraryDTOs
{
    public class EditPowerLibraryObject
    {
        public EditPowerLibraryAttributes AttributesActivatedLibrary { get; set; }
        public List<AddDdynamicAttributeInstallationValueViewModel> DynamicAttributes { get; set; }
        public AddLogisticalViewModel LogisticalItems { get; set; }
        public class EditPowerLibraryAttributes
        {
            public int Id { get; set; }
            public string Model { get; set; }
            public string? Note { get; set; } = "";
            public string? FrequencyRange { get; set; } = "";
            public string? BandWidth { get; set; } = "";
            public string? ChannelBandWidth { get; set; } = "";
            public string? Type { get; set; } = "";
            public float Size { get; set; } = 0;
            public string? L_W_H { get; set; } = "";
            public float width { get; set; } = 0;
            public float Weight { get; set; } = 0;
            public float Length { get; set; } = 0;
            public float Height { get; set; } = 0;
            public float Depth { get; set; } = 0;
            public float SpaceLibrary { get; set; }
            public bool Active { get; set; } = true;
            public bool Deleted { get; set; } = false;
        }
    }
}
