using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;
using TLIS_DAL.ViewModels.LogisticalDTOs;

namespace TLIS_DAL.ViewModels.CivilWithLegLibraryDTOs
{
    public class EditCivilWithLegsLibraryObject
    {
        public EditCivilWihtLegsLibraryAttributes attributesActivatedLibrary { get; set; }
        public List<AddDdynamicAttributeInstallationValueViewModel> dynamicAttributes { get; set; }
        public AddLogisticalViewModel logisticalItems { get; set; }




        public class EditCivilWihtLegsLibraryAttributes
        {   
            public int Id { get; set; }
            public string Model { get; set; }
            public string? Note { get; set; }
            public string Prefix { get; set; }
            public float Height_Designed { get; set; } = 0;
            public float Max_load_M2 { get; set; } = 0;
            public float SpaceLibrary { get; set; } = 0;
            public bool Active { get; set; } 
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
