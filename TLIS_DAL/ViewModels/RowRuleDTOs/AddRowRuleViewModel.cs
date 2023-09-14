using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.ViewModels.RuleDTOs;

namespace TLIS_DAL.ViewModels.RowRuleDTOs
{
    public class AddRowRuleViewModel
    {
        public AddInstRuleViewModel Rule { get; set; }
        public int? LogicalOperationId { get; set; }
    }
}
