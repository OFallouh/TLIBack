using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModelBase
{
    public class HistoryViewModel
    {
        public string Key { get; set; }
        public string Operation { get; set; }
        public string User { get; set; }
        public DateTime Date { get; set; }
    }
}
