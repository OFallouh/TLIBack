using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.ViewModels.DependencyRowDTOs;

namespace TLIS_DAL.ViewModels.DependencyDTOs
{
    public class DependencyViewModel
    {
        public List<AddDependencyRowViewModel> DependencyRows { get; set; }
        public int? OperationId { get; set; }
        public string ValueString { get; set; }
        public double? ValueDouble { get; set; }
        public DateTime? ValueDateTime { get; set; }
        public bool? ValueBoolean { get; set; }
    }
}
