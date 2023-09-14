using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.ViewModels.RowRuleDTOs;

namespace TLIS_DAL.ViewModels.DependencyRowDTOs
{
    public class AddDependencyRowViewModel
    {
        public List<AddRowRuleViewModel> RowRules { get; set; }
        public int? LogicalOperationId { get; set; }
    }
}
