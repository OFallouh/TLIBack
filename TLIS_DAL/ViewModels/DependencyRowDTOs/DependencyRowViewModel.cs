using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModels.DependencyRowDTOs
{
    public class DependencyRowViewModel
    {
        public int Id { get; set; }
        public int? DependencyId { get; set; }
        public int? RowId { get; set; }
        public int? LogicalOperationId { get; set; }
    }
}
