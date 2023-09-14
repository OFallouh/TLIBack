using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.ViewModels.RuleDTOs;

namespace TLIS_DAL.ViewModels.DependencyDTOs
{
    public class DynamicAttributeDependency
    {
        public int Id { get; set; }
        public int OperationId { get; set; }
        public string OperationName { get; set; }
        public object Value { get; set; }
        public List<DynamicAttributeDependencyRule> Rules { get; set; }
    }
}
