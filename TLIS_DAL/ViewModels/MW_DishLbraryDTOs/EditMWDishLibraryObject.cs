using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TLIS_DAL.ViewModels.MW_BULibraryDTOs.EditMWBULibraryObject;
using static TLIS_DAL.ViewModels.SideArmLibraryDTOs.EditSideArmLibraryObject;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;
using System.ComponentModel.DataAnnotations.Schema;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.LogisticalDTOs;

namespace TLIS_DAL.ViewModels.MW_DishLbraryDTOs
{
    public class EditMWDishLibraryObject
    {
        public EditMWDishLibraryAttributes AttributesActivatedLibrary { get; set; }
        public List<AddDdynamicAttributeInstallationValueViewModel> DynamicAttributes { get; set; }
        public AddLogisticalViewModel LogisticalItems { get; set; }
        public class EditMWDishLibraryAttributes
        {
            public int Id { get; set; }
            public string Model { get; set; }
            public string? Description { get; set; } = " ";
            public string? Note { get; set; } = " ";
            public float Weight { get; set; } = 0;
            public string? dimensions { get; set; } = " ";
            public float Length { get; set; } = 0;
            public float Width { get; set; } = 0;
            public float Height { get; set; } = 0;
            public float diameter { get; set; } = 0;
            public string? frequency_band { get; set; } = " ";
            public float SpaceLibrary { get; set; } = 0;
            public bool Active { get; set; } 
            public bool Deleted { get; set; } = false;    
            public int polarityTypeId { get; set; }         
            public int asTypeId { get; set; }
        }
     }
}
