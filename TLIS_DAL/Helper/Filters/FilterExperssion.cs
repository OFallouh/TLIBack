using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.Helper.Filters
{
    public class FilterExperssion
    {
        public string propertyName { get; set; }
        public string comparison { get; set; }
        public List<string> value { get; set; }
        public FilterExperssion() { }
        public FilterExperssion(string propertyName , string comparison , List<string> value)
        {
            this.propertyName = propertyName;
            this.comparison = comparison;
            this.value = value;
        }
    }
    public class FilterExperssionOneValue
    {
        public string propertyName { get; set; }
        public string comparison { get; set; }
        public string value { get; set; }
        public FilterExperssionOneValue() { }
        public FilterExperssionOneValue(string propertyName, string comparison, string value)
        {
            this.propertyName = propertyName;
            this.comparison = comparison;
            this.value = value;
        }
    }
}
