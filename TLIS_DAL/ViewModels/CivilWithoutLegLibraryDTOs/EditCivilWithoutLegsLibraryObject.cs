using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TLIS_DAL.ViewModels.CivilWithLegLibraryDTOs.EditCivilWithLegsLibraryObject;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;
using TLIS_DAL.ViewModels.LogisticalDTOs;
using static TLIS_DAL.ViewModels.CivilWithoutLegLibraryDTOs.AddCivilWithoutLegsLibraryObject;

namespace TLIS_DAL.ViewModels.CivilWithoutLegLibraryDTOs
{
    public class EditCivilWithoutLegsLibraryObject
    {
        public EditCivilWihtoutLegsLibraryAttributes attributesActivatedLibrary { get; set; }
        public List<AddDdynamicAttributeInstallationValueViewModel> dynamicAttributes { get; set; }
        public AddLogisticalViewModel logisticalItems { get; set; }
        public class EditCivilWihtoutLegsLibraryAttributes
        {
            public int Id { get; set; }
            public string Model { get; set; }
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
            public int? CivilWithoutLegCategoryId { get; set; }
            public float Manufactured_Max_Load { get; set; }
            public int structureTypeId { get; set; }
            public string? WidthVariation { get; set; }
        }
    }
}
