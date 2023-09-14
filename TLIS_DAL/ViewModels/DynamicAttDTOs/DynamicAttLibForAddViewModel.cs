using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.ViewModelBase;

namespace TLIS_DAL.ViewModels.DynamicAttDTOs
{
    public class DynamicAttLibForAddViewModel
    {
        public List<BaseAttView> AttributesActivated { get; set; } = new List<BaseAttView>();

        public List<KeyValuePair<string, List<DropDownListFilters>>> RelatedTables { get; set; } = new List<KeyValuePair<string, List<DropDownListFilters>>>();
    }
}
