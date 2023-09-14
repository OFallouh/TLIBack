using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModelBase
{
    public class ReturnWithFilters<TModel>
    {
        public IEnumerable<TModel> Model { get; set; }
        public IEnumerable<KeyValuePair<string,List<DropDownListFilters>>> filters { get; set; } 
    }
}
