using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;

namespace TLIS_DAL.ViewModels.CivilWithoutLegDTOs
{
    public class GetForAddCivilWithOutLegInstallationcs
    {
        public IEnumerable<BaseInstAttViews> LibraryAttribute { get; set; } = new List<BaseInstAttViews>();

        public IEnumerable<BaseInstAttViews> InstallationAttributes { get; set; } = new List<BaseInstAttViews>();

        public IEnumerable<BaseInstAttViews> CivilSiteDate { get; set; } = new List<BaseInstAttViews>();

        public IEnumerable<BaseInstAttViews> CivilSupportDistance { get; set; } = new List<BaseInstAttViews>();
        public IEnumerable<BaseInstAttViewDynamic> DynamicAttribute { get; set; } = new List<BaseInstAttViewDynamic>();
    }
}
