using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;
using TLIS_DAL.ViewModels.DynamicAttDTOs;

namespace TLIS_DAL.ViewModels.CivilWithLegLibraryDTOs
{
    public class GetForAddCivilLibrarybject
    {
        public IEnumerable<BaseInstAttViews> AttributesActivatedLibrary { get; set; } = new List<BaseInstAttViews>();
        public IEnumerable<BaseInstAttViews> LogisticalItems { get; set; } = new List<BaseInstAttViews>();
        public IEnumerable<BaseInstAttViewDynamic> DynamicAttributes { get; set; } = new List<BaseInstAttViewDynamic>();
    }
}
