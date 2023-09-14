using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.Helper.Filters
{
    public class Filter
    {
        public string key { get; set; }
        public List<string> value { get; set; }
    }

    public class FilterObject
    {
        public string key { get; set; }
        public object value2 { get; set; }
        public string op { get; set; }
        public FilterObject (string key , object value)
        {
            this.key = key;
            this.value2 = value;
        }
        public FilterObject(string key, object value, string op)
        {
            this.key = key;
            this.value2 = value;
            this.op = op;
        }
    }

    public class FilterObjectList
    {
        //private string v1;
        //private bool v2;

        public string key { get; set; }
        public List<object> value { get; set; }
      
        public FilterObjectList()
        { }
        public FilterObjectList(string key, List<object> value)
        {
            this.key = key;
            this.value = value;
        }
    }

    // class to convert list<object> to list<string>...
    public class StringFilterObjectList
    {
        //private string v1;
        //private bool v2;

        public string key { get; set; }
        public List<string> value { get; set; }

        public StringFilterObjectList()
        { }
        public StringFilterObjectList(string key, List<string> value)
        {
            this.key = key;
            this.value = value;
        }
    }
}
