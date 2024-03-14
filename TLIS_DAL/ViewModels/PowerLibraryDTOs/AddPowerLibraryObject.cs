using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;
using TLIS_DAL.ViewModels.LogisticalDTOs;

namespace TLIS_DAL.ViewModels.PowerLibraryDTOs
{
    public class AddPowerLibraryObject
    {
        public PowerLibraryAttributes LibraryAttribute { get; set; }
        public AddLogisticalViewModel LogisticalItems { get; set; }
        public List<AddDdynamicAttributeInstallationValueViewModel> dynamicAttribute { get; set; }
        public class PowerLibraryAttributes
        {
            public string Model { get; set; }
            public string Note { get; set; }
            public float Weight { get; set; }
            public string FrequencyRange { get; set; }
            public string Type { get; set; }
            public float Size { get; set; }
            public string L_W_H { get; set; }
            public float width { get; set; }
            public float Length { get; set; }
            public float Depth { get; set; }
            public float SpaceLibrary { get; set; }
            public bool Active { get; set; }
            public bool Deleted { get; set; }
        }
    }
}
