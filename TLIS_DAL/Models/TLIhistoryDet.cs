using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLIS_DAL.Models
{
    public class TLIhistoryDet
    {
        public int Id { get; set; }
        public int HistoryId { get; set; }
        public TLIhistory History { get; set; }
        public string RecordId { get; set; }
        public int TablesNameId { get; set; }
        public TLItablesNames TablesName { get; set; }
        public string? AttributeName { get; set; }
        public AttributeType AttributeType { get; set; }
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
    }
}
