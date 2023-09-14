using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.ViewModels.DataTypeDTOs;
using TLIS_DAL.ViewModels.LogicalOperationDTOs;
using TLIS_DAL.ViewModels.OperationDTOs;

namespace TLIS_DAL.ViewModels.DynamicAttDTOs
{
    public class FirstStepAddDependencyViewModel
    {
        public FirstStepAddDependencyViewModel()
        {
            Operations = new List<OperationViewModel>();
            LogicalOperations = new List<LogicalOperationViewModel>();
            DataTypes = new List<DataTypeViewModel>();
        }
        public List<OperationViewModel> Operations { get; set; }
        public List<LogicalOperationViewModel> LogicalOperations { get; set; }
        public List<DataTypeViewModel> DataTypes { get; set; }
        public int TableNameId { get; set; }
    }
}
