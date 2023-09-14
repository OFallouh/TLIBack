using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.ViewModels.CivilWithLegDTOs;
using TLIS_DAL.ViewModels.CivilWithoutLegDTOs;
using TLIS_DAL.ViewModels.MW_BULibraryDTOs;
using TLIS_DAL.ViewModels.MW_DishLbraryDTOs;
using TLIS_DAL.ViewModels.MW_ODUDTOs;

namespace TLIS_DAL.ViewModels.LogisticalitemDTOs
{
    public class LogisticalitemViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? CivilId { get; set; }
        public bool IsLib { get; set; }
        public int? logisticalId { get; set; }
        public IEnumerable<CivilWithLegLibraryViewModel> civilWithLeg { get; set; }
        public IEnumerable<CivilWithoutLegLibraryViewModel> civilWithoutLeg { get; set; }
        public IEnumerable<MW_BULibraryViewModel> MW_BU { get; set; }
        public IEnumerable<MW_DishLibraryViewModel> MW_Dish { get; set; }
        public IEnumerable<MW_ODULibraryViewModel> MW_ODU { get; set; }

    }
}
