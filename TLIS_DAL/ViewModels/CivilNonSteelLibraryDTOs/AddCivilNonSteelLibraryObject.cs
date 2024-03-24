using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;
using TLIS_DAL.ViewModels.LogisticalDTOs;

namespace TLIS_DAL.ViewModels.CivilNonSteelLibraryDTOs
{
    public class AddCivilNonSteelLibraryObject
    {
        public CivilNonSteelLibraryAttributes attributesActivatedLibrary { get; set; }
        public List<AddDdynamicAttributeInstallationValueViewModel> dynamicAttributes { get; set; }
        public AddLogisticalViewModel logisticalItems { get; set; }
        public class CivilNonSteelLibraryAttributes
        {
            public string Note { get; set; }
            public float Hight { get; set; } = 0;
            public float SpaceLibrary { get; set; } = 0;
            public string Prefix { get; set; }
            public bool VerticalMeasured { get; set; } = false;
            public int civilNonSteelTypeId { get; set; }
            public float NumberofBoltHoles { get; set; } = 0;
            public bool Active { get; set; } = true;
            public bool Deleted { get; set; } = false;
            public float Manufactured_Max_Load { get; set; } = 0;
        }
    }
}
