using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;

namespace TLIS_DAL.ViewModels.DynamicAttDTOs
{
    public class GetForAddDynamicAttribute
    {
        public IEnumerable<BaseInstAttViews> AttributesActivated { get; set; } = new List<BaseInstAttViews>();
        public IEnumerable<BaseInstAttViewDynamic> DynamicAttributes { get; set; } = new List<BaseInstAttViewDynamic>();
        public IEnumerable<BaseInstAttViews> Operation { get; set; } = new List<BaseInstAttViews>();
    }
}
