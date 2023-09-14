using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModels.RuleDTOs
{
    public class RuleViewModel
    {
        public int Id { get; set; }
        public int attributeActivatedId { get; set; }
        public int? OperationId { get; set; }
        public string OperationValue { get; set; }
    }
}
