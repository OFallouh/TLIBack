using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.ViewModels.DynamicAttDTOs;

namespace TLIS_DAL.ViewModelBase
{
    public class ObjectAttributeInst
    {
        public IEnumerable<BaseAttView> LibraryActivatedAttributes { get; set; } = new List<BaseAttView>();

        public IEnumerable<BaseInstAttView> AttributesActivated { get; set; } = new List<BaseInstAttView>();


        public IEnumerable<BaseInstAttView> CivilLoads { get; set; } = new List<BaseInstAttView>();

       
        public IEnumerable<DynaminAttInstViewModel> DynamicAtts { get; set; } = new List<DynaminAttInstViewModel>();

        public IEnumerable<KeyValuePair<string, List<DropDownListFilters>>> RelatedTables { get; set; } = new List<KeyValuePair<string, List<DropDownListFilters>>>();
    }
}
