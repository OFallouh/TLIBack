using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.ViewModels.CabinetDTOs;

namespace TLIS_DAL.ViewModels.PowerDTOs
{
    public class PowerLoadDto
    {
        public PowerViewModel Power { get; set; }
        public PowerLibraryViewModel PowerLibrary { get; set; }
        public IEnumerable<DynamicAttDto> DynamicAttList { get; set; }
    }
}
