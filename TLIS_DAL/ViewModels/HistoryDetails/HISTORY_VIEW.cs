using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLIS_DAL.ViewModels.HistoryDetails
{
    public class HISTORY_VIEW
    {
        public string? Note { get; set; }
        public string? BASE_TABLE { get; set; }
        public string? SECONDARY_TABLE { get; set; }
        public string? UserName { get; set; }

        // تم تعديل النوع إلى int؟ بدلاً من decimal؟
        public int? USER_ID { get; set; }

        public string? SysName { get; set; }

        // تم تعديل النوع إلى int؟ بدلاً من decimal؟
        public int? SYS_ID { get; set; }

        public DateTime? HistoryDate { get; set; }
        public string? Operation { get; set; }

        // تم تعديل النوع إلى int؟ بدلاً من decimal؟
        public int? PreviousHistoryId { get; set; }

        public string? BASE_RECORD_ID { get; set; }
        public string? SITECODE { get; set; }
        public string? SECONDARY_RECORD_ID { get; set; }
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public string? AttributeName { get; set; }

        // تم تعديل النوع إلى int؟ بدلاً من decimal؟ لأن الفيو يقوم بإرجاع INT
        public int? AttributeType { get; set; }

        public int? Id { get; set; }
    }



}
