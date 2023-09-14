using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModels.RuleDTOs
{
    public class AddInstRuleViewModel
    {
        public string TableName { get; set; }
        public int? CategoryId { get; set; }
        public int? attributeActivatedId { get; set; }
        public int? dynamicAttId { get; set; }
        public int? OperationId { get; set; }
        public string OperationValueString { get; set; }
        public double? OperationValueDouble { get; set; }
        public DateTime? OperationValueDateTime { get; set; }
        public bool? OperationValueBoolean { get; set; }
        public bool IsDynamic { get; set; }
    }
}
