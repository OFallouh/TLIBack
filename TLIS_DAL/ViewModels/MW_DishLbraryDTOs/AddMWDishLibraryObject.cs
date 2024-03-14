using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public string Description { get; set; }
        public string Note { get; set; }
        public float Weight { get; set; }
        public string dimensions { get; set; }
        public float Length { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public float diameter { get; set; }
        public string frequency_band { get; set; }
        public float SpaceLibrary { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }
        public int polarityTypeId { get; set; }
        public int asTypeId { get; set; }
        
        }
    }
}
