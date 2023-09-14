using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModels.StepActionDTOs
{
    public class EditStepActionConditionViewModel
    {
        public int Id { get; set; }
        public int ActionId { get; set; }
        public int Period { get; set; }
        public string label { get; set; }
        public StepActionGroupViewModel StepActionGroup { get; set; }
        public List<AddStepActionOptionConditionViewModel> StepActionOption { get; set; }
    }
}
