using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.ViewModels.CabinetDTOs;
using TLIS_DAL.ViewModels.MW_BULibraryDTOs;

namespace TLIS_DAL.ViewModels.MW_BUDTOs
{
    public class MWBULoadDto
    {
        public MW_BUViewModel MW_BU { get; set; }
        public MW_BULibraryViewModel MW_BULibrary { get; set; }
        public IEnumerable<DynamicAttDto> DynamicAttList { get; set; }
    }
}
