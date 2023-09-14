using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModels.StepActionDTOs
{
    public class AddStepActionApplyCalculationViewModel
    {
        public int WorkflowId { get; set; }
        public int Period { get; set; }
        public string label { get; set; }
        public bool CalculateLandSpace { get; set; }
        public bool CalculateLoadSpace { get; set; }
        public List<int> NextStepActions { get; set; }
    }
}
