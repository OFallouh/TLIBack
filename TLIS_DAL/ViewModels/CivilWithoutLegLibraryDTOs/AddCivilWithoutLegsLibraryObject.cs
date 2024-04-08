using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;
using TLIS_DAL.ViewModels.LogisticalDTOs;

namespace TLIS_DAL.ViewModels.CivilWithoutLegLibraryDTOs
{
    public class AddCivilWithoutLegsLibraryObject
    {
        public CivilWihtoutLegsLibraryAttributes attributesActivatedLibrary { get; set; }
        public List<AddDdynamicAttributeInstallationValueViewModel> dynamicAttributes { get; set; }
        public AddLogisticalViewModel logisticalItems { get; set; }
        public class CivilWihtoutLegsLibraryAttributes
        {
            public string? Note { get; set; }
            public float Height_Designed { get; set; } = 0;
            public float HeightBase { get; set; } = 0;
            public float Max_Load { get; set; } = 0;
            public float SpaceLibrary { get; set; } = 0;
            public string Prefix { get; set; }
            public bool Active { get; set; } = true;
            public bool Deleted { get; set; } = false;
            public int? CivilSteelSupportCategoryId { get; set; }
            public int? InstCivilwithoutLegsTypeId { get; set; }
            public int CivilWithoutLegCategoryId { get; set; }
            public float Manufactured_Max_Load { get; set; }
            public int structureTypeId { get; set; }
        }
    }
}
