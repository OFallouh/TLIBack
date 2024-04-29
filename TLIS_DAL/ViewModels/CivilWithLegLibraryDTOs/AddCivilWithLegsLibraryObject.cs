using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;
using TLIS_DAL.ViewModels.DynamicAttDTOs;
using TLIS_DAL.ViewModels.LogisticalDTOs;

namespace TLIS_DAL.ViewModels.CivilWithLegLibraryDTOs
{
    public class AddCivilWithLegsLibraryObject
    {
        public CivilWihtLegsLibraryAttributes attributesActivatedLibrary { get; set; }
        public List<AddDdynamicAttributeInstallationValueViewModel> dynamicAttributes { get; set; }
        public AddLogisticalViewModel logisticalItems { get; set; }




        public class CivilWihtLegsLibraryAttributes
        {
            public string Model { get; set; }
            public string? Note { get; set; }
            public string Prefix { get; set; }
            public float Height_Designed { get; set; } = 0;
            public float Max_load_M2 { get; set; } = 0;
            public float SpaceLibrary { get; set; } = 0;
            public bool Active { get; set; } = true;
            public bool Deleted { get; set; } = false;
            public int supportTypeDesignedId { get; set; }
            public int sectionsLegTypeId { get; set; }
            public int structureTypeId { get; set; }
            public int? civilSteelSupportCategoryId { get; set; }
            public float Manufactured_Max_Load { get; set; } = 0;
            public string? WidthVariation { get; set; }
            public int NumberOfLegs { get; set; }
        }
    }

}
