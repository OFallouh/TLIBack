using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLIS_DAL.Models
{
    public class TLIhistory
    {
        public int Id { get; set; }
        public string? RecordId { get; set; }
        public int TablesNameId { get; set; }
        public TLItablesNames TablesName { get; set; }
        public int HistoryTypeId { get; set; }
        public TLIhistoryType HistoryType { get; set; }
        public DateTime HistoryDate { get; set; }
        public int? PreviousHistoryId { get; set; }
        public TLIuser User { get; set; }
        public int? UserId { get; set; }
        public TLIexternalSys ExternalSys { get; set; }
        public int? ExternalSysId { get; set; }
        public string? SiteCode { get; set; }
    }
}
