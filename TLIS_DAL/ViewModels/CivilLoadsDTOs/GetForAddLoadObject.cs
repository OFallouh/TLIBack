using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;
using TLIS_DAL.ViewModels.DynamicAttDTOs;

namespace TLIS_DAL.ViewModels.CivilLoadsDTOs
{
    public class GetForAddLoadObject
    {
        public IEnumerable<BaseInstAttViews> installationConfig { get; set; } = new List<BaseInstAttViews>();
        public IEnumerable<BaseInstAttViews> LibraryAttribute { get; set; } = new List<BaseInstAttViews>();
        public IEnumerable<BaseInstAttViews> InstallationAttributes { get; set; } = new List<BaseInstAttViews>();
        public IEnumerable<BaseInstAttViews> CivilLoads { get; set; } = new List<BaseInstAttViews>();
        public IEnumerable<DynaminAttInstViewModel> dynamicAttribute { get; set; } = new List<DynaminAttInstViewModel>();
    }
}
