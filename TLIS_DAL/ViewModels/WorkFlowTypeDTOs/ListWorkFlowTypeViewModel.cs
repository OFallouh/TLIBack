using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModels.WorkFlowTypeDTOs
{
    public class ListWorkFlowTypeViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int WorkFlowId { get; set; }
        public int? nextStepActionId { get; set; }
    }
}
