using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.Helper.Filters
{
    public class DateFilterViewModel
    {
        public string key { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
    }
}
