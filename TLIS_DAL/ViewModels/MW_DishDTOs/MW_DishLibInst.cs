using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.CabinetDTOs;
using TLIS_DAL.ViewModels.MW_DishLbraryDTOs;

namespace TLIS_DAL.ViewModels.MW_DishDTOs
{
    public class MW_DishLibInst
    {
        public MW_DishViewModel MW_Dish { get; set; }
        public MW_DishLibraryViewModel MW_DishLibrary { get; set; }
        public IEnumerable<DynamicAttDto> DynamicAttList { get; set; }
        public int? ItemStatusId { get; set; }
        public TLIitemStatus ItemStatus { get; set; }
        public int? TicketId { get; set; }

    }
}
