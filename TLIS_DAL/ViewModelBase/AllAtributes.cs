using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.ViewModels.DynamicAttDTOs;

namespace TLIS_DAL.ViewModelBase
{
    public class AllAtributes
    {
        public IEnumerable<BaseAttView> AttributesActivated { get; set; } = new List<BaseAttView>();
        public IEnumerable<DynamicAttLibViewModel> DynamicAtts { get; set; } = new List<DynamicAttLibViewModel>();
    }
}
