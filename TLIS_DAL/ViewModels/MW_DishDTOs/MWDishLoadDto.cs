using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.ViewModels.CabinetDTOs;
using TLIS_DAL.ViewModels.MW_DishLbraryDTOs;

namespace TLIS_DAL.ViewModels.MW_DishDTOs
{
    public class MWDishLoadDto
    {
        public MW_DishViewModel MW_Dish { get; set; }
        public MW_DishLibraryViewModel MW_DishLibrary { get; set; }
        public IEnumerable<DynamicAttDto> DynamicAttList { get; set; }
    }
}
