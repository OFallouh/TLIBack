using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModelBase
{
    public class EditHistoryViewModel
    {
        public int id { get; set; }
        public string User { get; set; }
        public DateTime date { get; set; }
        public string HistoryType { get; set; }
        public int recordId { get; set; }
    }
}
