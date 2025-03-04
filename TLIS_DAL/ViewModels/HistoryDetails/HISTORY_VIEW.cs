using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLIS_DAL.ViewModels.HistoryDetails
{
    public class HISTORY_VIEW
    {
        public string Note { get; set; }
        public string BASE_TABLE { get; set; }
        public string SECONDARY_TABLE { get; set; }
        public string UserName { get; set; }
        public int? USER_ID { get; set; }  // يجب أن يكون Nullable
        public string SysName { get; set; }
        public int? SYS_ID { get; set; }  // يجب أن يكون Nullable
        public DateTime? HistoryDate { get; set; }  // يجب أن يكون Nullable
        public string Operation { get; set; }
        public int? PreviousHistoryId { get; set; }  // يجب أن يكون Nullable
        public string BASE_RECORD_ID { get; set; }  // يجب أن يكون string لأنه قد يحتوي على NULL
        public string SITECODE { get; set; }
        public string SECONDARY_RECORD_ID { get; set; }  // يجب أن يكون string لأنه قد يحتوي على NULL
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public string AttributeName { get; set; }
        public decimal? AttributeType { get; set; }  // يجب أن يكون Nullable
        public int Id { get; set; }
    }


}
