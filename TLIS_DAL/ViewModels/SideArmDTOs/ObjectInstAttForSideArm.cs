using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.DynamicAttDTOs;

namespace TLIS_DAL.ViewModels.SideArmDTOs
{
    public class ObjectInstAttForSideArm
    {
        public IEnumerable<BaseAttView> LibraryActivatedAttributes { get; set; } = new List<BaseAttView>();

        public IEnumerable<BaseInstAttView> AttributesActivated { get; set; } = new List<BaseInstAttView>();

        public IEnumerable<DynaminAttInstViewModel> DynamicAtts { get; set; } = new List<DynaminAttInstViewModel>();


    }
}
