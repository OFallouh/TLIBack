using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModels.StaticAttributesHistory
{
    public class StaticAttsHistoryViewModel
    {
        public string Key { get; set; }
        public string UpdatedInfo { get; set; }
        public string Operation { get; set; }
        public string User { get; set; }
        public DateTime Date { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
    }
}
