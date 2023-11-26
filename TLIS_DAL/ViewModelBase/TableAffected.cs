using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLIS_DAL.ViewModelBase
{
    public class TableAffected
    {
        public string TableName { get; set; }
        public bool isLibrary { get; set; }
        public List<RecordAffected> RecordsAffected { get; set; }
    }
}
