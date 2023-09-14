using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModels.RuleDTOs
{
    public class DynamicAttributeDependencyRule
    {
        public int Id { get; set; }
        public string AttributeName { get; set; }
        public string RuleOperation { get; set; }
        public object RuleValue { get; set; }
        public string TableName { get; set; }
    }
}
