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
        public IEnumerable<BaseAttViews> AttributesActivatedLibrary { get; set; } = new List<BaseAttViews>();
        public IEnumerable<BaseInstAttViewDynamic> DynamicAtts { get; set; } = new List<BaseInstAttViewDynamic>();
    }
}
