using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;
using TLIS_DAL.ViewModels.LogisticalDTOs;

namespace TLIS_DAL.ViewModels.RadioRRULibraryDTOs
{
    public class AddRadioRRULibraryObject
    {
        public RadioRRULibraryAttributes LibraryAttribute { get; set; }
        public AddLogisticalViewModel LogisticalItems { get; set; }
        public List<AddDdynamicAttributeInstallationValueViewModel> dynamicAttribute { get; set; }
        public class RadioRRULibraryAttributes
        {   
            public string Model { get; set; }
            public string Type { get; set; }
            public string Band { get; set; }
            public float? ChannelBandwidth { get; set; }
            public float? Weight { get; set; }
            public string L_W_H_cm3 { get; set; }
            public float Length { get; set; }
            public float Width { get; set; }
            public float Height { get; set; }
            public string Notes { get; set; }
            public float SpaceLibrary { get; set; }
            public bool Active { get; set; }
            public bool Deleted { get; set; }

        }
    }
}
