using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;
using TLIS_DAL.ViewModels.DynamicAttDTOs;
using TLIS_DAL.ViewModels.LogisticalDTOs;

namespace TLIS_DAL.ViewModels.MW_RFULibraryDTOs
{
    public class AddMWRFULibraryObject
    {
        public MWRFULibraryAttributes LibraryAttribute { get; set; }
        public AddLogisticalViewModel LogisticalItems { get; set; }
        public List<AddDdynamicAttributeInstallationValueViewModel> dynamicAttribute { get; set; }
        public class MWRFULibraryAttributes
        {
            public string Model { get; set; }
            public string Note { get; set; }
            public float? Weight { get; set; }
            public string L_W_H { get; set; }
            public float Length { get; set; }
            public float Width { get; set; }
            public float Height { get; set; }
            public string size { get; set; }
            public string tx_parity { get; set; }
            public string frequency_band { get; set; }
            public float SpaceLibrary { get; set; }
            public bool Active { get; set; }
            public bool Deleted { get; set; }
            public int? diversityTypeId { get; set; } = 0;
            public int? boardTypeId { get; set; } = 0;
            public string FrequencyRange { get; set; }
            public string RFUType { get; set; }
            public string VenferBoardName { get; set; }
        }
    }
}
