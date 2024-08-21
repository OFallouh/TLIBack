using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLIS_DAL.ViewModels.DynamicAttDTOs
{
    public class AddDynamicObject
    {
        public GeneralObject general { get; set; }
        public ValidationObject validation { get; set; }
        public DependencyObject dependency { get; set; }
        public int? type { get; set; }
    }

    public class GeneralObject
    {
        public string name { get; set; }
        public string? description { get; set; }
        public bool isRequired { get; set; }
        public object defualtValue { get; set; }
        public int? dataType { get; set; }
    }
    public class ValidationObject
    {
        public int? operation { get; set; }
        public object value { get; set; }


    }
    public class DependencyObject
    {
        public List<List<GroupObject>> groups { get; set; }
        public object? result { get; set; }


    }
    public class GroupObject
    {
        public string? ColumnName { get; set; }
        public int? Operation { get; set; }
        public object Value { get; set; }

    }
   
}

