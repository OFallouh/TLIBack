using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModelBase
{
    public class BassAttViewModel
    {
        public string Key { get; set; }
        public object Value { get; set; }
        public string Desc { get; set; }
        public string Label { get; set; }
        public bool? Manage { get; set; }
        public bool Required { get; set; }
        public bool? enable { get; set; }
        public bool? AutoFill { get; set; }
        public string DataType { get; set; }
        public string? DefaultValue { get; set; }

    }
}
