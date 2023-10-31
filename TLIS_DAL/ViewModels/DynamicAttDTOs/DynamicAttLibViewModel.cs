using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModels.DynamicAttDTOs
{
    public class DynamicAttLibViewModel
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public int? DataTypeId { get; set; }
        public string DataType { get; set; }
        public object Value { get; set; }
        public bool Required { get; set; }
        public bool Active { get; set; }

    }
}
