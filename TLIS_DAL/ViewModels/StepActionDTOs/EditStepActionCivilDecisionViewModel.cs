using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.ViewModels.ItemStatusDTOs;

namespace TLIS_DAL.ViewModels.StepActionDTOs
{
    public class EditStepActionCivilDecisionViewModel
    {
        public int Id { get; set; }
        public int Period { get; set; }
        public string label { get; set; }
        public List<ListStepActionItemStatusWiNameViewModel> IncomItemStatus { get; set; }
        public StepActionGroupViewModel StepActionGroup { get; set; }
        public List<AddStepActionOptionConditionViewModel> StepActionOption { get; set; }
        public List<AddStepActionItemOptionDecisionViewModel> StepActionItemOption { get; set; }
        //public List<AddStepActionItemStatusViewModel> StepActionItemStatus { get; set; }
    }
}
