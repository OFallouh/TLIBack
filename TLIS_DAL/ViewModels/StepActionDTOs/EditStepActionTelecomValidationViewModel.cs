using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModels.StepActionDTOs
{
    public class EditStepActionTelecomValidationViewModel
    {
        public int Id { get; set; }
        public string label { get; set; }
        public int Period { get; set; }
        public List<ListStepActionItemStatusWiNameViewModel> IncomItemStatus { get; set; }
        public StepActionGroupViewModel StepActionGroup { get; set; }
        public List<AddStepActionPartFileViewModel> StepActionPart { get; set; }
        public List<AddStepActionItemOptionValidationViewModel> StepActionItemOption { get; set; }
    }
}
