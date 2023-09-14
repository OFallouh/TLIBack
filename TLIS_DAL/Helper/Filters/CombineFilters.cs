using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.Helper.Filters
{
    public class CombineFilters
    {
        public List<FilterObjectList> filters { get; set; }
        public List<DateFilterViewModel> DateFilter { get; set; }
    }
}
