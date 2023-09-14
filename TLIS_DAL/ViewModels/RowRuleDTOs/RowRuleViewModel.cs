using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModels.RowRuleDTOs
{
    public class RowRuleViewModel
    {
        public int Id { get; set; }
        public int? RuleId { get; set; }
        public int? RowId { get; set; }
        public int? LogicalOperationId { get; set; }
    }
}
