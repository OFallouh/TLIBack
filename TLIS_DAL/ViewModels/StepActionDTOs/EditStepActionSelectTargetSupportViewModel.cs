using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModels.StepActionDTOs
{
    public class EditStepActionSelectTargetSupportViewModel
    {
        public int Id { get; set; }
        public int Period { get; set; }
        public string label { get; set; }
        public StepActionGroupViewModel StepActionGroup { get; set; }
        public List<int> NextStepActions { get; set; }
    }
}
