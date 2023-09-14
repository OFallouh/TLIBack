using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.ViewModels.CabinetDTOs;
using TLIS_DAL.ViewModels.GeneratorLibraryDTOs;

namespace TLIS_DAL.ViewModels.GeneratorDTOs
{
    public class GeneratorDisplayedOnTableViewModel
    {
        public GeneratorViewModel Generator { get; set; }
        public GeneratorLibraryViewModel GeneratorLibrary { get; set; }
        public IEnumerable<DynamicAttDto> DynamicAttList { get; set; }
    }
}
