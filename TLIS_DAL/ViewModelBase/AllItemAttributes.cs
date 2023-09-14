using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.AttributeActivatedDTOs;
using TLIS_DAL.ViewModels.DynamicAttDTOs;
using TLIS_DAL.ViewModels.DynamicAttInstValueDTOs;
using TLIS_DAL.ViewModels.DynamicAttLibValueDTOs;

namespace TLIS_DAL.ViewModelBase
{
    public class AllItemAttributes
    {
        public IEnumerable<BaseAttView> AttributesActivated { get; set; } = new List<BaseAttView>();
        public IEnumerable<DynamicAttLibViewModel> DynamicAtts { get; set; } = new List<DynamicAttLibViewModel>();
        public IEnumerable<DynaminAttInstViewModel> DynamicAttInst { get; set; } = new List<DynaminAttInstViewModel>();
    }
}
