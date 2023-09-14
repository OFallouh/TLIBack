using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.ViewModels.CabinetDTOs;
using TLIS_DAL.ViewModels.GeneratorDTOs;
using TLIS_DAL.ViewModels.SolarDTOs;

namespace TLIS_DAL.ViewModels.SiteDTOs
{
    public class SiteOtherInventoriesViewModel
    {
        public List<CabinetViewModel> cabinets { get; set; }
        public List<SolarViewModel> Solars { get; set; }
        public List<GeneratorViewModel> Generators { get; set; }
    }
}
