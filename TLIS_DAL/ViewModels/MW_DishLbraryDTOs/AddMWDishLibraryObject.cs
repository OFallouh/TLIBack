using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;
using TLIS_DAL.ViewModels.LogisticalDTOs;

namespace TLIS_DAL.ViewModels.MW_DishLbraryDTOs
{
    public class AddMWDishLibraryObject
    {
        public MWDishLibraryAttributes LibraryAttribute { get; set; }
        public AddLogisticalViewModel LogisticalItems { get; set; }
        public List<AddDdynamicAttributeInstallationValueViewModel> dynamicAttribute { get; set; }
        public class MWDishLibraryAttributes
        {
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
            public bool Active { get; set; } = true;
            public bool Deleted { get; set; } = false;
            public int polarityTypeId { get; set; }
            public int asTypeId { get; set; }
          

        }
    }
}
