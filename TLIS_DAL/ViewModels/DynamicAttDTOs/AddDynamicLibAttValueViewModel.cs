using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModels.DynamicAttDTOs
{
    public class AddDynamicLibAttValueViewModel
    {
        public int DynamicAttId { get; set; }
        //public string Key { get; set; }
        //public bool LibraryAtt { get; set; }
        //public int? DataTypeId { get; set; }
        public string ValueString { get; set; }
        public double? ValueDouble { get; set; }
        public DateTime? ValueDateTime { get; set; }
        public bool? ValueBoolean { get; set; }
        //public int? dynamicListValuesId { get; set; }
    }
}
