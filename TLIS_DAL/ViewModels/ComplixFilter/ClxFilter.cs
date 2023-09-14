using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.Helper.Filters;

namespace TLIS_DAL.ViewModels.ComplixFilter
{
    public class ClxFilter
    {
        public List<SimpleFilter>? Filters { get; set; } = new List<SimpleFilter>();
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 5;
    }
}
