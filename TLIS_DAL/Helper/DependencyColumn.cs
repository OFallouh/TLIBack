using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.ViewModelBase;

namespace TLIS_DAL.Helper
{
    public class DependencyColumn
    {
        public int? RecordId { get; set; }
        public string columnName { get; set; }
        public string columnType { get; set; }
        public bool IsDynamic { get; set; }
        public List<DropDownListFilters> value { get; set; }

        public DependencyColumn(string colName , string colType,bool isdynamic,List<DropDownListFilters> val=null, int? recordId = null)
        {
            RecordId = recordId;
            columnName = colName;
            columnType = colType;
            value = val;
            IsDynamic = isdynamic;
        }
    }
}
