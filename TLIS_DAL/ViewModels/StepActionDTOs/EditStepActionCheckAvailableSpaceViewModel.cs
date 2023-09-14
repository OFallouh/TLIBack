using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModels.StepActionDTOs
{
    public class EditStepActionCheckAvailableSpaceViewModel
    {
        public int Id { get; set; }
        public string label { get; set; }
        public int Period { get; set; }
        public List<AddStepActionOptionDecisionViewModel> StepActionOption { get; set; }
    }
}
