using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.ViewModels.CabinetDTOs;
using TLIS_DAL.ViewModels.MW_BUDTOs;
using TLIS_DAL.ViewModels.MW_BULibraryDTOs;

namespace TLIS_DAL.ViewModels.MW_ODUDTOs
{
    public class MWODULoadDto
    {
        public MW_ODUViewModel MW_ODU { get; set; }
        public MW_ODULibraryViewModel MW_ODULibrary { get; set; }
        public IEnumerable<DynamicAttDto> DynamicAttList { get; set; }
    }
}
