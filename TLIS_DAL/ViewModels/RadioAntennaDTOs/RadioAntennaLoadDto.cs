using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.ViewModels.CabinetDTOs;
using TLIS_DAL.ViewModels.RadioAntennaLibraryDTOs;

namespace TLIS_DAL.ViewModels.RadioAntennaDTOs
{
    public class RadioAntennaLoadDto
    {
        public RadioAntennaViewModel RadioAntenna { get; set; }
        public RadioAntennaLibraryViewModel RadioAntennaLibrary { get; set; }
        public IEnumerable<DynamicAttDto> DynamicAttList { get; set; }
    }
}
