using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;
using TLIS_DAL.ViewModels.LogisticalDTOs;

namespace TLIS_DAL.ViewModels.MW_OtherLibraryDTOs
{
    public class AddMWOtherLibraryObject
    {
        public MWOtherLibraryAttributes LibraryAttribute { get; set; }
        public AddLogisticalViewModel LogisticalItems { get; set; }
        public List<AddDdynamicAttributeInstallationValueViewModel> dynamicAttribute { get; set; }
        public class MWOtherLibraryAttributes
        {
            public string Model { get; set; }
            public string Note { get; set; }
            public float Length { get; set; }
            public float Width { get; set; }
            public float Height { get; set; }
            public string L_W_H { get; set; }
            public string frequency_band { get; set; }
            public float SpaceLibrary { get; set; }
            public bool Active { get; set; }
            public bool Deleted { get; set; }
        }
    }
}
