using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModels.StepActionDTOs
{
    public class AddStepActionCheckAvailableSpaceViewModel
    {
        public int WorkflowId { get; set; }
        public int Period { get; set; }
        public string label { get; set; }
        public List<AddStepActionOptionDecisionViewModel> StepActionOption { get; set; }
        //public List<int> NextStepActions { get; set; }

    }
}
