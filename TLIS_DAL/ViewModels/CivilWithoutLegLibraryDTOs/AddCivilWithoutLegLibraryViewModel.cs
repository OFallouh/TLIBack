using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;
using TLIS_DAL.ViewModels.DynamicAttDTOs;
using TLIS_DAL.ViewModels.DynamicAttInstValueDTOs;
using TLIS_DAL.ViewModels.LogisticalDTOs;
using TLIS_DAL.ViewModels.LogisticalitemDTOs;
using static TLIS_DAL.ViewModels.CivilWithLegDTOs.AddCivilWithLegLibraryViewModel;

namespace TLIS_DAL.ViewModels.CivilWithoutLegDTOs
{
    public class AddCivilWithoutLegLibraryViewModel
    {
        public CivilWihtoutLegsLibraryAttributes LibraryAttribute { get; set; }
        public AddLogisticalViewModel LogisticalItems { get; set; }
        public List<AddDdynamicAttributeInstallationValueViewModel> dynamicAttribute { get; set; }
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
            public int CivilSteelSupportCategoryId { get; set; }
            public int? InstCivilwithoutLegsTypeId { get; set; }
            public int? CivilWithoutLegCategoryId { get; set; }
            public float Manufactured_Max_Load { get; set; }
            public int structureTypeId { get; set; }
        }
    }
}
