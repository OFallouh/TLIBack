using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.ViewModels.CabinetDTOs;
using TLIS_DAL.ViewModels.MW_BUDTOs;
using TLIS_DAL.ViewModels.MW_BULibraryDTOs;

namespace TLIS_DAL.ViewModels.MW_RFUDTOs
{
    public class MWRFULoadDto
    {
        public MW_RFUViewModel MW_RFU { get; set; }
        public MW_RFULibraryViewModel MW_RFULibrary { get; set; }
        public IEnumerable<DynamicAttDto> DynamicAttList { get; set; }
    }
}
