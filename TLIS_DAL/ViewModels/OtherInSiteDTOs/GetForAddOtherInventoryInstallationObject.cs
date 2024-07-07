using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;

namespace TLIS_DAL.ViewModels.OtherInSiteDTOs
{
    public class GetForAddOtherInventoryInstallationObject
    {
        public IEnumerable<BaseInstAttViews> LibraryAttribute { get; set; } = new List<BaseInstAttViews>();
        public IEnumerable<BaseInstAttViews> InstallationAttributes { get; set; } = new List<BaseInstAttViews>();
        public IEnumerable<BaseInstAttView> OtherInSite { get; set; } = new List<BaseInstAttView>();
        public IEnumerable<BaseInstAttView> OtherInventoryDistance { get; set; } = new List<BaseInstAttView>();
        public IEnumerable<BaseInstAttViewDynamic> DynamicAttribute { get; set; } = new List<BaseInstAttViewDynamic>();
    }
}
