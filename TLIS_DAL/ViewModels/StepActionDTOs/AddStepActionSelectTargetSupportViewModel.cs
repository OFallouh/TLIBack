using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModels.StepActionDTOs
{
    public class AddStepActionSelectTargetSupportViewModel
    {
        public int WorkflowId { get; set; }
        public int Period { get; set; }
        public string label { get; set; }
        public StepActionGroupViewModel StepActionGroup { get; set; }
        public List<int> NextStepActions { get; set; }
    }
}
