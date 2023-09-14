using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.ViewModels.CivilWithLegDTOs;
using TLIS_DAL.ViewModels.DynamicAttDTOs;
using TLIS_DAL.ViewModels.DynamicAttInstValueDTOs;

namespace TLIS_DAL.ViewModels.CivilWithLegLibraryDTOs
{
    public class AddCivilWithLegLibraryWithDynamicAttsViewModel
    {
        public AddCivilWithLegLibraryViewModel CivilWithLegLibrary { get; set; }
        public List<AddDynamicLibAttValueViewModel> DynamicLibAttsValue { get; set; }
        public List<AddDynamicAttInstValueViewModel> DynamicAttsInstValue { get; set; }
    }
}
